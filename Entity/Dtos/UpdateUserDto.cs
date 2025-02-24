namespace RunningApplicationNew.Entity.Dtos
{


    public class UpdateUserDto
    {
        
        public int? Age { get; set; } // Optional
        public double? Height { get; set; } // Optional
        public double? Weight { get; set; } // Optional
        public string? Gender { get; set; } // Optional
        public int? Active { get; set; }
        public int? RunPrefer { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public DateTime? Birthday { get; set; }


    }
}
