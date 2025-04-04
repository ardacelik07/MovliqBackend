﻿using RunningApplicationNew.Entity;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.IRepository;

namespace RunningApplicationNew.RepositoryLayer
{
    public interface IRaceRoomRepository : IRepository<RaceRoom>
    {
        Task<List<RaceRoom>> GetActiveRoomsAsync();
        Task<List<RaceRoom>> GetRacesRoomsAsync();
        Task<List<RaceRoom>> GetAllRoomsAsync();
        Task<RaceRoom> CreateRoomAsync(DateTime startTime,string type,int duration);
        Task AddUserToRoomAsync(int userId, int raceRoomId);
        Task RemoveUserFromRoomAsync(int userId, int raceRoomId);
        Task<int> GetRoomParticipantsCountAsync(int raceRoomId);
        Task<List<User>> GetRoomParticipantsAsync(int roomId);
        Task<List<string>> GetRoomParticipantNamesAsync(int roomId);
        Task<List<RaceRoom>> GetActiveRoomsAsyncByType(string type);
        Task<User> GetRoomParticipantByEmailAsync(int roomId, string email);
        Task SetRoomInactiveAsync(int roomId);
        Task UpdateUserStatsAsync(int userId, int roomId, double distance, int steps);
        Task ResetUserStatsOnLeaveAsync(int userId, int roomId);
        Task<List<RoomParticipantDto>> GetRoomParticipantsWithProfilesAsync(int roomId);
        
    }
}
