using RunningApplicationNew.Entity;
using RunningApplicationNew.IRepository;

namespace RunningApplicationNew.RepositoryLayer
{
    public interface IRaceRoomRepository : IRepository<RaceRoom>
    {
        Task<List<RaceRoom>> GetActiveRoomsAsync();
        Task<RaceRoom> CreateRoomAsync(DateTime startTime,string type);
        Task AddUserToRoomAsync(int userId, int raceRoomId);
        Task<int> GetRoomParticipantsCountAsync(int raceRoomId);
        Task<List<User>> GetRoomParticipantsAsync(int roomId);
        Task<List<RaceRoom>> GetActiveRoomsAsyncByType(string type);
        Task<User> GetRoomParticipantByEmailAsync(int roomId, string email);
        Task SetRoomInactiveAsync(int roomId);


    }
}
