using RunningApplicationNew.Entity;

namespace RunningApplicationNew.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);



            

    }
}
