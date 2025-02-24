using Microsoft.EntityFrameworkCore;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Entity;
using RunningApplicationNew.IRepository;
using System.Threading.Tasks;

namespace RunningApplicationNew.DataLayer
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        // Email'e göre kullanıcıyı alırken, sadece aktif olan kullanıcıları döndür
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive); // Sadece aktif kullanıcıyı döndür
        }


        // Soft delete işlemi: Kullanıcının aktifliğini false yap
        public async Task DeactivateUserAsync(User user)
        {
            user.IsActive = false; // Kullanıcıyı devre dışı bırak
            _context.Update(user);  // Değişikliği kaydet
            await _context.SaveChangesAsync();  // Veritabanına kaydet
        }


    }
}
