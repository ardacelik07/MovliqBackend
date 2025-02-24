using RunningApplicationNew.Entity;
using RunningApplicationNew.IRepository;

namespace RunningApplicationNew.RepositoryLayer
{
    public interface IUserResultsRepository : IRepository<UserResults>
    {
        Task<List<string>> GetRoomNamesByEmailAsync(string email);
    }
}
