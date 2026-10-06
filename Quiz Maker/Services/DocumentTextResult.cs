namespace Quiz_Maker.Services
{
    public class DocumentTextResult
    {
        public bool Success { get; set; }
        public string Text { get; set; }
        public string? Error { get; set; }
        public bool WasTruncated { get; set; }
    }
}
