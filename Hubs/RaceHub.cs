using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using RunningApplicationNew.RepositoryLayer;
using RunningApplicationNew.IRepository;
using RunningApplicationNew.Entity;
using Azure.Core;


namespace RunningApplicationNew.Hubs
{
    [Authorize]
    public class RaceHub : Hub
    {
        private readonly IRaceRoomRepository _raceRoomRepository;
        private readonly IUserRepository _userRepository;
        
        public RaceHub(IRaceRoomRepository raceRoomRepository,IUserRepository userRepository)
        {
            _raceRoomRepository = raceRoomRepository;
            _userRepository = userRepository;
        }

        // Odaya katılma
        public async Task JoinRoom(int roomId)
        {
            // Kullanıcının email'ini al
            var email = Context.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetByEmailAsync(email);

            // Odaya ekle
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{roomId}");

            // Diğer kullanıcılara bildir
            await Clients.OthersInGroup($"room-{roomId}").SendAsync("UserJoined", user.UserName);

            // Odadaki tüm katılımcıları al
            var participants = await _raceRoomRepository.GetRoomParticipantsWithProfilesAsync(roomId);

            // Sadece yeni kullanıcıya mevcut katılımcıları gönder
            // await Clients.Caller.SendAsync("RoomParticipants", participants);
            await Clients.Group($"room-{roomId}").SendAsync("RoomParticipants", participants);
        }

        // Odadan ayrılma
        public async Task LeaveRoom(int roomId)
        {
            var email = Context.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetByEmailAsync(email);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{roomId}");
            await Clients.Group($"room-{roomId}").SendAsync("UserLeft", user.UserName);
        }
        public async Task LeaveRoomDuringRace(int roomId)
        {
            var email = Context.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetByEmailAsync(email);
            await _raceRoomRepository.ResetUserStatsOnLeaveAsync(user.Id, roomId);
            await _raceRoomRepository.RemoveUserFromRoomAsync(user.Id, roomId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{roomId}");
            await Clients.Group($"room-{roomId}").SendAsync("UserLeft", user.UserName);
        }

        // Konum güncelleme
        public async Task UpdateLocation(int roomId, double distance, int steps)
        {
            // Kullanıcının email'ini al
            var email = Context.User.FindFirstValue(ClaimTypes.Email);

            var user = await _userRepository.GetByEmailAsync(email);

            await _raceRoomRepository.UpdateUserStatsAsync(user.Id, roomId, distance, steps);
            
            // Kullanıcı bilgilerini güncelle ve sıralamayı hesapla
            await Clients.Group($"room-{roomId}").SendAsync("LocationUpdated", email, distance, steps);
        }
    }
} 