namespace RunningApplicationNew.Entity
{
    public class UserRaceRoom
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RaceRoomId { get; set; }
        public RaceRoom RaceRoom { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
