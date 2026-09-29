using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Quiz_Maker.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }
        [Range(0, 100)]
        public int FreeAttemptsUsed { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}
