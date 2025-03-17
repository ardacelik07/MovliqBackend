using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection; // Yeni eklendi
using RunningApplicationNew.Hubs;
using RunningApplicationNew.IRepository;
using System.Linq;
using RunningApplicationNew.RepositoryLayer;
using RunningApplicationNew.Entity;
using RunningApplicationNew.Entity.Dtos;

namespace RunningApplicationNew.Services
{
    public class RaceUpdateService : BackgroundService
    {
        private readonly IHubContext<RaceHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory; // Repository yerine bunu kullanıyoruz
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        public RaceUpdateService(
            IHubContext<RaceHub> hubContext,
            IServiceScopeFactory scopeFactory) // Parametre değişti
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("RaceUpdateService başlatıldı..."); // Log eklendi

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Her döngüde yeni bir scope oluştur
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Scope içinden repository'leri al
                        var raceRoomRepository = scope.ServiceProvider.GetRequiredService<IRaceRoomRepository>();
                        var userResultsRepository = scope.ServiceProvider.GetRequiredService<IUserResultsRepository>();
                        var LeaderBoardRepository = scope.ServiceProvider.GetRequiredService<ILeaderBoardRepository>();

                        Console.WriteLine("Aktif yarışlar taranıyor..."); // Log eklendi

                        // Aktif yarışları bul
                        var activeRooms = await raceRoomRepository.GetRacesRoomsAsync();
                        Console.WriteLine($"Bulunan aktif oda sayısı: {activeRooms.Count()}"); // Log eklendi

                        foreach (var room in activeRooms)
                        {
                            Console.WriteLine($"Oda {room.Id} işleniyor... Başlangıç: {room.StartTime}, Süre: {room.Duration} dk"); // Log eklendi

                            // Yarış süresi dolmuş mu kontrol et
                            if (DateTime.Now > room.StartTime.AddMinutes(room.Duration))
                            {
                                var userResults = new RaceResultDto
                                {
                                    roomId = room.Id,
                                    RoomDuration = room.Duration,
                                    RoomName = room.RoomName,
                                    RoomType = room.Type,
                                    startTime = room.StartTime

                                };


                                await LeaderBoardRepository.UpdateLeaderBoardAsync(room.Id, room.Type);
                                await userResultsRepository.AddUserRacesResults(userResults);
                                await raceRoomRepository.SetRoomInactiveAsync(room.Id);
                                await _hubContext.Clients.Group($"room-{room.Id}").SendAsync("RaceEnded", room.Id);
                               
                                Console.WriteLine($"Oda {room.Id} yarışı sona erdi"); // Log eklendi
                                continue;
                            }

                            // Odadaki katılımcıları al ve sıralamayı güncelle
                            var participants = await raceRoomRepository.GetRoomParticipantsAsync(room.Id);
                            Console.WriteLine($"Oda {room.Id} katılımcı sayısı: {participants.Count}"); // Log eklendi

                            var results = participants
                                .Select(p => new {
                                    User = p,
                                    Distance = p.distancekm ?? 0,
                                    Steps = p.steps ?? 0
                                })
                                .OrderByDescending(r => r.Distance)
                                .ThenByDescending(r => r.Steps)
                                .Select((r, index) => new {
                                    r.User.Email,
                                    r.User.UserName,
                                    r.Distance,
                                    r.Steps,
                                    Rank = index + 1
                                })
                                .ToList();

                            // SignalR ile tüm kullanıcılara güncel sıralamayı gönder
                            Console.WriteLine($"Oda {room.Id} için liderlik tablosu gönderiliyor..."); // Log eklendi
                            await _hubContext.Clients.Group($"room-{room.Id}").SendAsync("LeaderboardUpdated", results);
                            Console.WriteLine($"Oda {room.Id} için liderlik tablosu gönderildi."); // Log eklendi
                        }
                    } // scope burada sonlanır
                }
                catch (Exception ex)
                {
                    // Hata detaylı olarak logla
                    Console.WriteLine($"Yarış güncelleme hatası: {ex.Message}");
                    Console.WriteLine($"Hata detayı: {ex.StackTrace}");
                }

                Console.WriteLine($"İşlem tamamlandı. {_updateInterval.TotalSeconds} saniye bekleniyor..."); // Log eklendi
                await Task.Delay(_updateInterval, stoppingToken);
            }
        }
    }
}