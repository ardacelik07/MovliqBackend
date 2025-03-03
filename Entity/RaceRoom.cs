namespace RunningApplicationNew.Entity
{
    public class RaceRoom
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public int Duration { get; set; }
        public int Status { get; set; } = 1; //1 Waiting, 2 Running, 3 Finished 

        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MinParticipants { get; set; } = 3;
        public int MaxParticipants { get; set; } = 3;
        public bool IsActive { get; set; } = true; // Oda aktif mi?
        public DateTime StartTime { get; set; }
    }
}
