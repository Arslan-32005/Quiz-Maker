using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz_Maker.Services;

namespace Quiz_Maker.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IDocumentTextExtractor _documentTextExtractor;
        private readonly IQuizGenerator _quizGenerator;

        public QuizController(IDocumentTextExtractor documentTextExtractor, IQuizGenerator quizGenerator)
        {
            _documentTextExtractor = documentTextExtractor;
            _quizGenerator = quizGenerator;
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
            var result = await _documentTextExtractor.ExtractAsync(file);

            if (result.Success)
            {
                ViewBag.Generation = await _quizGenerator.GenerateAsync(
                    result.Text, 5, HttpContext.RequestAborted);
            }

            return View(result);
        }
    }
}