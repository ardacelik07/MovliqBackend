using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.Entity;
using RunningApplicationNew.Helpers;
using System;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using RunningApplicationNew.RepositoryLayer;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using RunningApplicationNew.Hubs;

namespace RunningApplicationNew.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RaceRoomController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IRaceRoomRepository _raceRoomRepository;
        private readonly IUserResultsRepository _userResultsRepository;
        private readonly IHubContext<RaceHub> _hubContext;
        private readonly IJwtHelper _jwtHelper;
        private readonly IEmailHelper _emailhelper;

        public RaceRoomController(IUserRepository userRepository, IRaceRoomRepository raceRoomRepository, IJwtHelper jwtHelper, IEmailHelper emailHelper, IUserResultsRepository userResultsRepository, IHubContext<RaceHub> hubContext)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailhelper = emailHelper;
            _raceRoomRepository = raceRoomRepository;
            _userResultsRepository = userResultsRepository;
            _hubContext = hubContext;
        }
        [HttpPost("createRoom")]
        public async Task CreateRaceRoom([FromQuery] string roomType, int duration)
        {
            var nextStartTime = DateTime.Now; 

            var room = await _raceRoomRepository.CreateRoomAsync(nextStartTime, roomType,duration);

                

                await _raceRoomRepository.SaveChangesAsync();
            
        }
        [HttpGet("GetAllRooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            

            var room = await _raceRoomRepository.GetActiveRoomsAsync();

            return Ok(room);
   

        }

        [HttpGet("GetRoom/{roomType}")]
        public async Task<IActionResult> GetRoom(string roomType)
        {


            var room = await _raceRoomRepository.GetActiveRoomsAsyncByType(roomType);

            return Ok(room);


        }
        [HttpGet("join-room/{roomId}")]
        [Authorize]
        public async Task<IActionResult> JoinRoom(int roomId)
        {
            var room = await _raceRoomRepository.GetByIdAsync(roomId);
            if (room == null || !room.IsActive)
            {
                return BadRequest("Bu oda aktif değil.");
            }
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Token'dan alınan email ile kullanıcıyı al
            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");
            
            var userinroom = await _raceRoomRepository.GetRoomParticipantByEmailAsync(roomId, user.Email);
            if (userinroom == null)
            {
                var participantsCount = await _raceRoomRepository.GetRoomParticipantsCountAsync(roomId);

                if (participantsCount < 2)
                {
                    await _raceRoomRepository.AddUserToRoomAsync(user.Id, roomId);
                    
                    // Eğer bu kişi odaya katıldıktan sonra oda dolduysa (2 kişi olduysa)
                    if (participantsCount + 1 == 2)
                    {
                        // Odayı hemen inactive yap
                        await _raceRoomRepository.SetRoomInactiveAsync(roomId);
                        
                        // 10 saniye sonra yarışı başlatacağımızı belirt
                        return Ok(new { 
                            message = "Odaya başarıyla katıldınız. Oda doldu, 10 saniye içinde yarış başlayacak!", 
                            countdownStarted = true,
                            roomId = roomId 
                        });
                    }
                    
                    return Ok(new { 
                        message = "Odaya başarıyla katıldınız.", 
                        countdownStarted = false,
                        roomId = roomId 
                    });
                }
                else
                {
                    return BadRequest("oda dolu");
                }
                
            }
            else
            {
                return BadRequest("zaten odadasın");
            }
        }




        [HttpGet("get-room-participants/{roomId}")]
        [Authorize]
        public async Task<IActionResult> GetRoomParticipants(int roomId)
        {
            // Odayı al
            var room = await _raceRoomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                return NotFound("Oda bulunamadı.");
            }


            // Katılımcıları al
            var participants = await _raceRoomRepository.GetRoomParticipantsAsync(roomId);
            var participantscount = await _raceRoomRepository.GetRoomParticipantsCountAsync(roomId);

            if (participants == null || !participants.Any())
            {
                return Ok("Bu odada henüz katılımcı yok.");
            }
            var response = new
            {
                RoomId = room.Id,
                RoomName = room.RoomName,
                Participants = participants.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Email,
                    p.ProfilePicturePath,
                    p.Rank,
                    p.steps,
                    p.distancekm
                })
            };

            // Katılımcıları döndür


            if (!response.Participants.Any())
            {
                return Ok(new
                {
                    RoomId = room.Id,
                    RoomName = room.RoomName,
                    Message = "Bu odada henüz katılımcı yok."
                });
            }
           

            return Ok(response);
        }
        [HttpPost("get-User-final-Results")]
        [Authorize]
        public async Task<IActionResult> GetRoomParticipant(UserResultsDto results)
        {
            // Odayı al
            var room = await _raceRoomRepository.GetByIdAsync(results.roomId);
            if (room == null)
            {
                return NotFound("Oda bulunamadı.");
            }
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Katılımcıları al
            var participant = await _raceRoomRepository.GetRoomParticipantByEmailAsync(results.roomId, emailFromToken);



            if (participant == null)
            {

                return NotFound("bu odada bu kullanıcı yok");
            }
            // aynı oda için iki kere result atmayı engelle
            var checkroom = await _userResultsRepository.GetRoomNamesByEmailAsync(participant.Email);
            if (checkroom.Contains(room.RoomName))
            {
                return BadRequest("Bu oda için zaten sonucun var.");
            }

            var userResults = new UserResults
            {
                RoomName = room.RoomName,
                UserName = participant.UserName,
                Email = participant.Email,
                steps = results.steps,
                distancekm = results.DistanceKm,
                Rank = results.Rank,
                StartTime = room.StartTime,
                RoomType = room.Type,
                
            };

            await _userResultsRepository.AddAsync(userResults);
            await _userResultsRepository.SaveChangesAsync();

            // Katılımcıları döndür




            return Ok(userResults);
        }
        [HttpGet("GetAllUserResults")]
        public async Task<IActionResult> GetAllUserResults()
        {


            var results = await _userResultsRepository.GetAllAsync();

            return Ok(results);


        }

        [HttpPost("match-room")]
        [Authorize]
        public async Task<IActionResult> MatchRoom([FromBody] RoomMatchRequestDto request)
        {
            try
            {
                // Kullanıcının email bilgisini JWT'den al
                var email = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userRepository.GetByEmailAsync(email);
                
                if (user == null)
                    return NotFound(new { message = "Kullanıcı bulunamadı." });
                    
                // Belirtilen kriterlere göre aktif bir oda ara
                var activeRooms = await _raceRoomRepository.GetActiveRoomsAsyncByType(request.RoomType);
                var eligibleRoom = activeRooms
                    .Where(r => r.Duration == request.Duration)
                    .Where(r => r.IsActive) // Henüz başlamamış odalar
                    .FirstOrDefault(r => _raceRoomRepository.GetRoomParticipantsCountAsync(r.Id).Result < r.MaxParticipants);
                    
                // Uygun bir oda bulundu mu?
                if (eligibleRoom != null)
                {
                    // Kullanıcıyı odaya ekle
                    await _raceRoomRepository.AddUserToRoomAsync(user.Id, eligibleRoom.Id);
                    
                    // Odadaki katılımcı sayısını kontrol et
                    int participantCount = await _raceRoomRepository.GetRoomParticipantsCountAsync(eligibleRoom.Id);
                    
                    // Oda dolu mu? Dolu ise yarışı başlat
                    if (participantCount == eligibleRoom.MinParticipants)
                    {
                        // Yarışı başlat (StartTime'ı şimdiden 10 saniye sonraya ayarla)
                        eligibleRoom.IsActive = false;
                        eligibleRoom.StartTime = DateTime.Now.AddSeconds(10); 
                        await _raceRoomRepository.SaveChangesAsync();
                        
                        // SignalR ile odadaki tüm kullanıcılara bildirim gönder
                        await _hubContext.Clients.Group($"room-{eligibleRoom.Id}").SendAsync("RaceStarting", eligibleRoom.Id, 10);

                    }
                    
                    return Ok(new { roomId = eligibleRoom.Id, startTime = eligibleRoom.StartTime });
                }
                else
                {
                    // Uygun oda bulunamadı, yeni oda oluştur
                    var startTime = DateTime.Now.AddMinutes(2); // 2 dakika bekleme süresi
                    var newRoom = await _raceRoomRepository.CreateRoomAsync(startTime, request.RoomType, request.Duration);
                    
                    // Kullanıcıyı yeni odaya ekle
                    await _raceRoomRepository.AddUserToRoomAsync(user.Id, newRoom.Id);
                    
                    return Ok(new { 
                        roomId = newRoom.Id, 
                        startTime = newRoom.StartTime,
                        message = "Yeni oda oluşturuldu ve bekleme salonuna alındınız." 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

    }
}
