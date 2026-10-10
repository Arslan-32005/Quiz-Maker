namespace Quiz_Maker.Services
{
    public class GeneratedQuestion
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; }=string.Empty;

    }
}
