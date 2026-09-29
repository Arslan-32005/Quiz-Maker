using System.ComponentModel.DataAnnotations;
namespace Quiz_Maker.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Required]
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
        [Required]
        [StringLength(500)]
        public string Text { get; set; }
        [Required]
        [StringLength(300)]
        public string Explanation { get; set; }
        [Range(1, 100)]
        public int Order { get; set; }
        public List<Option> Options { get; set; } = new List<Option>();
    }
}
