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
    }
}
