using System.ComponentModel.DataAnnotations;

namespace Quiz_Maker.Models
{
    public class GenerationLog
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}