

namespace RunningApplicationNew.Entity
{
    public class LeaderBoard
    {
        public int Id { get; set; }
        public double? GeneralDistance { get; set; }
        public int?  OutdoorSteps { get; set; }
        public int? IndoorSteps { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }



    }
}
 