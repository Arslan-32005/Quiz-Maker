using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz_Maker.Services;

namespace Quiz_Maker.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IDocumentTextExtractor _documentTextExtractor; 
            public QuizController(IDocumentTextExtractor documentTextExtractor)
        {
            _documentTextExtractor = documentTextExtractor;
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
           var result= await _documentTextExtractor.ExtractAsync(file);
            return View(result);
        }
    }
}
