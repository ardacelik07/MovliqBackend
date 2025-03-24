using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Entity;
using RunningApplicationNew.IRepository;
using Microsoft.EntityFrameworkCore;
using RunningApplicationNew.Entity.Dtos;

namespace RunningApplicationNew.RepositoryLayer
{
    public class LeaderBoardRepository : GenericRepository<LeaderBoard>, ILeaderBoardRepository
    {
        private readonly IRaceRoomRepository _raceRoomRepository;

        public LeaderBoardRepository(ApplicationDbContext context, IRaceRoomRepository raceRoomRepository) : base(context)
        {

            _raceRoomRepository = raceRoomRepository;

        }

        public async Task UpdateLeaderBoardAsync(int roomId, string raceType)
        {
            var participantlist = await _raceRoomRepository.GetRoomParticipantsAsync(roomId);

            foreach (var participant in participantlist)
            {
                // Kullan�c�n�n LeaderBoard kayd�n� al veya olu�tur
                var leaderBoard = await _context.Set<LeaderBoard>().FirstOrDefaultAsync(lb => lb.UserId == participant.Id);

                if (leaderBoard == null)
                {
                    leaderBoard = new LeaderBoard
                    {
                        UserId = participant.Id,
                        GeneralDistance = 0,
                        OutdoorSteps = 0,
                        IndoorSteps = 0,
                        //TotalRaces = 0,
                        //OutdoorRaces = 0,
                        //IndoorRaces = 0
                    };
                    _context.Set<LeaderBoard>().Add(leaderBoard);
                }

                // Yar�� tipine g�re g�ncelleme yap
                //leaderBoard.TotalRaces++;

                if (raceType.ToLower() == "outdoor")
                {
                   // leaderBoard.OutdoorRaces++;
                    leaderBoard.OutdoorSteps += participant.steps;
                    leaderBoard.GeneralDistance += participant.distancekm; // Sadece outdoor i�in mesafe ekle

                    
                }
                else if (raceType.ToLower() == "indoor")
                {
                    //leaderBoard.IndoorRaces++;
                    leaderBoard.IndoorSteps += participant.steps;
                    // Indoor yar��larda GeneralDistance'a ekleme yapma

                    
                }
                


            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaderBoardIndoorDto>> GetAllLeaderBoardIndoor()
        {
            return await _context.Set<LeaderBoard>()
                .Include(lb => lb.User) // User tablosuyla join
                .OrderByDescending(lb => lb.IndoorSteps) // Mesafeye g�re azalan s�ralama
                .Select(lb => new LeaderBoardIndoorDto
                {
                    Id = lb.Id,
                    UserId = lb.UserId,
                    UserName = lb.User.UserName,
                    profilePicture = lb.User.ProfilePicturePath,// User tablosundan username
                    IndoorSteps = lb.IndoorSteps
                })
                .ToListAsync();
        }

        public async Task<List<LeaderBoardOutdoorDto>> GetAllLeaderBoardOutdoor()
        {
            return await _context.Set<LeaderBoard>()
                .Include(lb => lb.User) // User tablosuyla join
                .OrderByDescending(lb => lb.GeneralDistance) // Mesafeye g�re azalan s�ralama
                .Select(lb => new LeaderBoardOutdoorDto
                {
                    Id = lb.Id,
                    UserId = lb.UserId,
                    UserName = lb.User.UserName,
                    profilePicture = lb.User.ProfilePicturePath,
                    OutdoorSteps = lb.OutdoorSteps,
                    GeneralDistance= lb.GeneralDistance// User tablosundan username
                    
                })
                .ToListAsync();
        }

        public async Task<LeaderBoardRankDto> GetLeaderboardRankById(int userId)
        {
            var user = await _context.Set<LeaderBoard>()
                .FirstOrDefaultAsync(lb => lb.UserId == userId);

            

            var indoorRank = await _context.Set<LeaderBoard>()
                .CountAsync(x => x.IndoorSteps > user.IndoorSteps) + 1;

            var outdoorRank = await _context.Set<LeaderBoard>()
                .CountAsync(x => x.GeneralDistance > user.GeneralDistance) + 1;

            return new LeaderBoardRankDto
            {
                IndoorRank = indoorRank,
                OutdoorRank = outdoorRank
            };
        }
    }
}
