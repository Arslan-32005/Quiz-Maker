namespace Quiz_Maker.Services
{
    public class QuizGenerationResult
    {
        public bool Success { get; set; }
        public GeneratedQuiz? Quiz { get; set; }
        public string? Error { get; set; }
    }
}
