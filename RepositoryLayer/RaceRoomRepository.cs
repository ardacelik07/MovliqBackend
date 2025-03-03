using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Entity;
using RunningApplicationNew.IRepository;
using Microsoft.EntityFrameworkCore;

namespace RunningApplicationNew.RepositoryLayer
{
    public class RaceRoomRepository : GenericRepository<RaceRoom>, IRaceRoomRepository
    {
        

        public RaceRoomRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<RaceRoom>> GetActiveRoomsAsync()
        {
            return await _context.Set<RaceRoom>().Where(r => r.Status==1).ToListAsync();
        }
        public async Task<List<RaceRoom>> GetRacesRoomsAsync()
        {
            return await _context.Set<RaceRoom>().Where(r => r.Status==2).ToListAsync();
        }
        public async Task<List<RaceRoom>> GetAllRoomsAsync()
        {
            return await _context.Set<RaceRoom>().ToListAsync();
        }
        public async Task<List<RaceRoom>> GetActiveRoomsAsyncByType(string type)
        {
            return await _context.Set<RaceRoom>()
                .Where(r => r.Status==1 && r.Type == type)
                .ToListAsync();
        }

        public async Task<RaceRoom> CreateRoomAsync(DateTime startTime, string type, int duration)
        {
            var room = new RaceRoom
            {
                RoomName = $"Room_{Guid.NewGuid()}",
                CreatedAt = DateTime.Now,
                StartTime = startTime,
                IsActive = true,
                Status = 1, // Beklemede
                Type = type,
                Duration = duration
                
            };

            _context.Set<RaceRoom>().Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task AddUserToRoomAsync(int userId, int raceRoomId)
        {
            var userRaceRoom = new UserRaceRoom
            {
                UserId = userId,
                RaceRoomId = raceRoomId,
                JoinedAt = DateTime.Now
            };

            _context.Set<UserRaceRoom>().Add(userRaceRoom);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetRoomParticipantsCountAsync(int raceRoomId)
        {
            return await _context.Set<UserRaceRoom>().CountAsync(u => u.RaceRoomId == raceRoomId);
        }
        public async Task<List<User>> GetRoomParticipantsAsync(int roomId)
        {
            return await _context.Set<UserRaceRoom>()
                                 .Where(rru => rru.RaceRoomId == roomId)
                                 .Select(rru => rru.User)
                                 .ToListAsync();
        }
        public async Task<User> GetRoomParticipantByEmailAsync(int roomId, string email)
        {
            return await _context.Set<UserRaceRoom>()
                                 .Where(rru => rru.RaceRoomId == roomId && rru.User.Email == email)
                                 .Select(rru => rru.User)
                                 .FirstOrDefaultAsync();
        }

        public async Task SetRoomInactiveAsync(int roomId)
        {
            var room = await _context.Set<RaceRoom>().FindAsync(roomId);
            if (room != null)
            {
                room.Status = 3; // Yarış bitti
                

                // Katılımcıların istatistiklerini sıfırla
                var participants = await GetRoomParticipantsAsync(roomId);
                foreach (var participant in participants)
                {
                    participant.distancekm = 0;
                    participant.steps = 0;
                    _context.Set<User>().Update(participant);
                }

                // TÜM DEĞİŞİKLİKLERİ BİR KEZ KAYDET
                await _context.SaveChangesAsync();

                Console.WriteLine($"Oda {roomId} inaktif yapıldı ve {participants.Count} kullanıcının istatistikleri sıfırlandı.");
            }
        }




        public async Task UpdateUserStatsAsync(int userId, int roomId, double distance, int steps)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user != null)
            {
                user.distancekm = distance;
                user.steps = steps;
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
            }
        }

    }
}
