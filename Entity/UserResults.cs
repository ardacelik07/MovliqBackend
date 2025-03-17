namespace RunningApplicationNew.Entity
{
    public class UserResults
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? RoomId { get; set; }


        public string? UserName { get; set; }

        

        public string? Email { get; set; }

        public string? RoomName { get; set; }


        public double? distancekm { get; set; }

        public double? steps { get; set; }

        public int? Rank { get; set; }


        public DateTime? StartTime { get; set; }

        public string? RoomType { get; set; }

        public int? Duration { get; set; }

        public int? Calories { get; set; }

        public int? avarageSpeed { get; set; }

        


    }
}
