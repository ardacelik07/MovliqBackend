namespace RunningApplicationNew.Entity.Dtos
{
    public class LeaderBoardOutdoorDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        
        public int? OutdoorSteps { get; set; }

        public double? GeneralDistance { get; set; }

        public string? profilePicture { get; set; }
    }
}
