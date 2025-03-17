namespace RunningApplicationNew.Entity.Dtos
{
    public class LeaderBoardIndoorDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int? IndoorSteps { get; set; }
        public string? profilePicture { get; set; }
    }
}
