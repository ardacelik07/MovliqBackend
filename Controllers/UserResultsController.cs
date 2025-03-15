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
    public class UserResultsController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IRaceRoomRepository _raceRoomRepository;
        private readonly IUserResultsRepository _userResultsRepository;
        private readonly IHubContext<RaceHub> _hubContext;
        private readonly IJwtHelper _jwtHelper;
        private readonly IEmailHelper _emailhelper;

        public UserResultsController(IUserRepository userRepository, IRaceRoomRepository raceRoomRepository, IJwtHelper jwtHelper, IEmailHelper emailHelper, IUserResultsRepository userResultsRepository, IHubContext<RaceHub> hubContext)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailhelper = emailHelper;
            _raceRoomRepository = raceRoomRepository;
            _userResultsRepository = userResultsRepository;
            _hubContext = hubContext;
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
                RoomId = room.Id,
                UserId = participant.Id,
                UserName = participant.UserName,
                Email = participant.Email,
                steps = results.steps,
                distancekm = results.DistanceKm,
                Rank = results.Rank,
                StartTime = room.StartTime,
                RoomType = room.Type,
                RoomName = room.RoomName,
                Duration = room.Duration
               
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

        [HttpPost("add-user-record")]
        [Authorize]
        public async Task<IActionResult> AddUserRecord(RecordRequestDto recordRequest)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            var user = await _userRepository.GetByEmailAsync(emailFromToken);



            var userResult = new UserResults
            {

                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                steps = recordRequest.Steps,
                distancekm = recordRequest.Distance,
                StartTime = recordRequest.startTime,
                RoomType = "record",
                Duration = recordRequest.Duration,
                Calories = recordRequest.Calories,
                avarageSpeed = recordRequest.AverageSpeed


            };
            await _userResultsRepository.AddAsync(userResult);
            await _userResultsRepository.SaveChangesAsync();

            return Ok(userResult);
        }

        [HttpGet("GetUserRecordResults")]
        public async Task<IActionResult> GetUserRecordResults()
        {


            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            

            var recordresults = await _userResultsRepository.GetUserRecordResult(emailFromToken);

            return Ok(recordresults);


        }

    }
}
