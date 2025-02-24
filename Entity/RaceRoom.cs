namespace RunningApplicationNew.Entity
{
    public class RaceRoom
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MinParticipants { get; set; } = 3;
        public int MaxParticipants { get; set; } = 3;
        public bool IsActive { get; set; } = false; // Oda aktif mi?
        public DateTime StartTime { get; set; }
    }
}
