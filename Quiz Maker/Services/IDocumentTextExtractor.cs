using Microsoft.AspNetCore.Http;
namespace Quiz_Maker.Services
{
    public interface IDocumentTextExtractor
    {
        public Task<DocumentTextResult> ExtractAsync(IFormFile file);
    }
}
