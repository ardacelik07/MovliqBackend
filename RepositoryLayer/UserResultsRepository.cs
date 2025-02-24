using Microsoft.EntityFrameworkCore;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Entity;
using RunningApplicationNew.IRepository;

namespace RunningApplicationNew.RepositoryLayer
{
    public class UserResultsRepository : GenericRepository<UserResults>, IUserResultsRepository
    {
        public UserResultsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<string>> GetRoomNamesByEmailAsync(string email)
        {
            var roomNames = await _context.Set<UserResults>()
                                          .Where(ur => ur.Email == email)
                                          .Select(ur => ur.RoomName)
                                          .Distinct()  // Aynı oda ismini bir kez döndürelim
                                          .ToListAsync();

            return roomNames;
        }

       
    }

}
