namespace Quiz_Maker.Models
{
    public class QuizResultViewModel
    {
        public Quiz Quiz { get; set; }
        public int Score { get; set; }
        public Dictionary<int, int> UserAnswers { get; set; }

    }
}
