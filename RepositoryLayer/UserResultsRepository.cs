using Microsoft.EntityFrameworkCore;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Entity;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.IRepository;

namespace RunningApplicationNew.RepositoryLayer
{
    public class UserResultsRepository : GenericRepository<UserResults>, IUserResultsRepository
    {
        private readonly IRaceRoomRepository _raceRoomRepository;

        public UserResultsRepository(ApplicationDbContext context,IRaceRoomRepository raceRoomRepository) : base(context) {
        
           _raceRoomRepository = raceRoomRepository;

        }

        public async Task<List<string>> GetRoomNamesByEmailAsync(string email)
        {
            var roomNames = await _context.Set<UserResults>()
                                          .Where(ur => ur.Email == email)
                                          .Select(ur => ur.RoomName)
                                          .Distinct()  // Aynı oda ismini bir kez döndürelim
                                          .ToListAsync();

            return roomNames;
        }

        public async Task<List<UserResults>> GetUserRecordResult(string email)
        {
            return await _context.Set<UserResults>()
                                .Where(ur => ur.Email == email && ur.RoomType == "record")
                                .ToListAsync();
        }


        public async Task AddUserRacesResults(RaceResultDto roominfos)
        {
           
              


                // Katılımcıların istatistiklerini sıfırla
                var participants = await  _raceRoomRepository.GetRoomParticipantsAsync(roominfos.roomId);
               
                foreach (var participant in participants)
                {

                var userResults = new UserResults
                {
                    RoomId = roominfos.roomId,
                    UserId = participant.Id,
                    UserName = participant.UserName,
                    Email = participant.Email,
                    steps = participant.steps,
                    distancekm = participant.distancekm,
                    Rank = participant.Rank,
                    StartTime = roominfos.startTime,
                    RoomType = roominfos.RoomType,
                    RoomName = roominfos.RoomName,
                    Duration = roominfos.RoomDuration

                };
                    _context.Set<UserResults>().Add(userResults);
                }

                // TÜM DEĞİŞİKLİKLERİ BİR KEZ KAYDET
                await _context.SaveChangesAsync();

               
            }
        }

    }


