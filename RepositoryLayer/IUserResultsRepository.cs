using RunningApplicationNew.Entity;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.IRepository;

namespace RunningApplicationNew.RepositoryLayer
{
    public interface IUserResultsRepository : IRepository<UserResults>
    {
        Task<List<string>> GetRoomNamesByEmailAsync(string email);
        Task<List<UserResults>> GetUserRecordResult(string email);
        Task AddUserRacesResults(RaceResultDto roominfos);
        Task<List<UserResults>> GetUserActivityResult(string email, string type, string period);
        Task<List<UserResults>> GetLastThreeActivities(string email);
        Task<int> UserStreakTrack(string email);
    }
}