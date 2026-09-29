using System.ComponentModel.DataAnnotations;
namespace Quiz_Maker.Models
{
    public class Option
    {
        public int Id { get; set; }
        [Required]
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        [Required]
        [StringLength(300)]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        [Range(1, 4)]
        public int Order { get; set; }
    }
}
