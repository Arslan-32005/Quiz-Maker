using System.ComponentModel.DataAnnotations;
namespace Quiz_Maker.Models
{
    public class Attempt
    {
        public int Id { get; set; }
        [Required]
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [StringLength(64)]
        public string? GuestToken { get; set; }
        [Range(0, 50)]
        public int Score { get; set; }
        [Range(1, 50)]
        public int TotalQuestions { get; set; }
        public DateTime AttemptedAt { get; set; }
    }
}
