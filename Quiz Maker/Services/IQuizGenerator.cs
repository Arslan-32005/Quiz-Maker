namespace Quiz_Maker.Services
{
    public interface IQuizGenerator
    {
        Task<QuizGenerationResult>GenerateAsync(string documentText,
            int QuestionCount, CancellationToken cancellationToken = default);
    }
}
