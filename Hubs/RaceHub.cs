using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using RunningApplicationNew.RepositoryLayer;
using RunningApplicationNew.IRepository;


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
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{roomId}");
            await Clients.Group($"room-{roomId}").SendAsync("UserJoined", Context.User.Identity.Name);
        }
        
        // Odadan ayrılma
        public async Task LeaveRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room-{roomId}");
            await Clients.Group($"room-{roomId}").SendAsync("UserLeft", Context.User.Identity.Name);
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