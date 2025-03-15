using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Entity;
using RunningApplicationNew.IRepository;
using Microsoft.EntityFrameworkCore;
using RunningApplicationNew.Entity.Dtos;

namespace RunningApplicationNew.RepositoryLayer
{
    public class LeaderBoardRepository : GenericRepository<LeaderBoard>, ILeaderBoardRepository
    {
        public LeaderBoardRepository(ApplicationDbContext context) : base(context) { }

    }     
}
