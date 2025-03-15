using System.ComponentModel.DataAnnotations;

namespace RunningApplicationNew.Entity
{
    public class User
    {
        [Key] // Primary key
        public int Id { get; set; }

        
        public string? Name { get; set; }

        
        public string? SurName { get; set; }

        public string? UserName { get; set; }

        [Required]
        [EmailAddress] // Email formatı kontrolü
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public int? Age { get; set; }


        public double? Height { get; set; }

        public double? Weight { get; set; }

       

        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; } // Şifreler hashlenmiş saklanır

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Kullanıcı oluşturulma tarihi
        public bool IsActive { get; set; }  // Kullanıcının aktiflik durumu

        public string? ProfilePicturePath { get; set; }

        public double? distancekm { get; set; }

        public double? steps { get; set; }

        public int? Rank { get; set; }

        public int? GeneralRank { get; set; }

        public string? Gender { get; set; }

        public int? Active { get; set; }

        public int Calories { get; set; }

        public int AverageSpeed { get; set; }

        public int? Runprefer { get; set; }

        public DateTime? Birthday { get; set; }





    }
}
