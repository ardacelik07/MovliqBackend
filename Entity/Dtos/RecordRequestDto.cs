namespace RunningApplicationNew.Entity.Dtos
{
    public class RecordRequestDto
    {
        public int? Duration { get; set; }
         
        public double? Distance { get; set; }

        public int ? Calories { get; set; }

        public double Steps { get; set; }

        public int ? AverageSpeed { get; set; }

        public DateTime? startTime { get; set; }
    }
}
