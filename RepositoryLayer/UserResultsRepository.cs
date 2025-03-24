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

        public UserResultsRepository(ApplicationDbContext context, IRaceRoomRepository raceRoomRepository) : base(context) {

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
            var participants = await _raceRoomRepository.GetRoomParticipantsAsync(roominfos.roomId);

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
        

        public async Task<List<UserResults>> GetUserActivityResult(string email, string type, string period)
        {
            // Bugünün tarihini al
            DateTime today = DateTime.Today;
            DateTime startDate;

            // period parametresine göre başlangıç tarihini belirle
            switch (period.ToLower())
            {
                case "weekly":
                    // Haftanın başlangıcı (Pazartesi)
                    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = today.AddDays(-diff);
                    break;

                case "monthly":
                    // Ayın başlangıcı
                    startDate = new DateTime(today.Year, today.Month, 1);
                    break;

                case "yearly":
                    // Yılın başlangıcı
                    startDate = new DateTime(today.Year, 1, 1);
                    break;

                default:
                    // Varsayılan olarak son 7 gün
                    startDate = today.AddDays(-7);
                    break;
            }

            return await _context.Set<UserResults>()
                .Where(ur => ur.Email == email &&
                             ur.RoomType == type &&
                             ur.StartTime >= startDate)
                .OrderBy(ur => ur.StartTime)
                .ToListAsync();
        }

        public async Task<List<UserResults>> GetLastThreeActivities(string email)
        {
            return await _context.Set<UserResults>()
                .Where(ur => ur.Email == email && ur.RoomType != "record") // RoomType "record" olmayanları al
                .OrderByDescending(ur => ur.StartTime) // En yeni aktiviteleri almak için sıralama
                .Take(3) // Son 3 aktiviteyi al
                .ToListAsync();
        }

        public async Task<int> UserStreakTrack(string email)
        {
            // Bugünün tarihini al
            DateTime today = DateTime.Today;

            // Bugüne ait yarışları kontrol et
            var todaysRaces = await _context.Set<UserResults>()
                .Where(ur => ur.Email == email &&
                             ur.StartTime >= today &&
                             ur.RoomType != "record")
                .ToListAsync();

            // Kullanıcının mevcut streak değerini veritabanından çek
            // Not: Bu bölüm veritabanı yapınıza göre uyarlanmalıdır
            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
            int currentStreak = user?.streak ?? 0;

            // Eğer bugün en az bir yarış varsa streak'i 1 artır, yoksa 0'la
            if (todaysRaces.Any())
            {
                currentStreak++;
            }
            else
            {
                currentStreak = 0;
            }

            // Streak değerini güncelle ve kaydet
            if (user != null)
            {
                user.streak = currentStreak;
                await _context.SaveChangesAsync();
            }

            return currentStreak;
        }

    } }


