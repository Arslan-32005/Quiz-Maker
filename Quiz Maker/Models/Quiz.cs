using System.ComponentModel.DataAnnotations;
namespace Quiz_Maker.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser?  User  { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [Required]
        [StringLength(12)]
        public string ShareCode { get; set; }
        [Required]
        [StringLength(255)]
        public string SourceFileName { get; set; }
        [Required]
        [StringLength(10)]
        public string SourceFileType { get; set; }
        [Range(1, 50)]
        public int TotalQuestions { get; set; }
        public bool IsPublic { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }

    
}
