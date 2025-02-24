namespace RunningApplicationNew.Entity.Dtos
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? Age { get; set; }
        public double? Height { get; set; }

        public double? Weight { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int?  Active { get; set; }
        public int?  RunPrefer { get; set; }
        public bool IsActive { get; set; }
        public string? ProfilePicturePath { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
