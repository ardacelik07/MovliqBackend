using RunningApplicationNew.Entity;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.IRepository;

namespace RunningApplicationNew.RepositoryLayer
{
    public interface ILeaderBoardRepository : IRepository<LeaderBoard>
    {

        Task UpdateLeaderBoardAsync(int roomId, string raceType);
        Task<List<LeaderBoardIndoorDto>> GetAllLeaderBoardIndoor();
        Task<List<LeaderBoardOutdoorDto>> GetAllLeaderBoardOutdoor();
    }
}
