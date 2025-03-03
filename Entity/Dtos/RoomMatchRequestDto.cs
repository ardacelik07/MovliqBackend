using System.ComponentModel.DataAnnotations;

namespace RunningApplicationNew.Entity.Dtos
{
    public class RoomMatchRequestDto
    {
        [Required]
        public string RoomType { get; set; }
        
        [Required]
        public int Duration { get; set; }
    }
} 