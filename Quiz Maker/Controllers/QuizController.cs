using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz_Maker.Services;
using Quiz_Maker.Models;
using Quiz_Maker.Config;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Quiz_Maker.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IDocumentTextExtractor _documentTextExtractor;
        private readonly IQuizGenerator _quizGenerator;
        private readonly QuizContext _quizContext;

        public QuizController(IDocumentTextExtractor documentTextExtractor, IQuizGenerator quizGenerator, QuizContext quizContext)
        {
            _documentTextExtractor = documentTextExtractor;
            _quizGenerator = quizGenerator;
            _quizContext = quizContext;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> Upload(IFormFile file, string? title, int totalQuestions)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                ModelState.AddModelError("", "User is not authenticated.");
                return View();
            }

            // Daily limit (Pakistan time, resets at midnight PKT)
            var pkt = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            var todayPkt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pkt).Date;
            var todayStartUtc = TimeZoneInfo.ConvertTimeToUtc(todayPkt, pkt);

            var todayCount = await _quizContext.Quizzes
                .CountAsync(q => q.UserId == userId && q.CreatedAt >= todayStartUtc);
            if (todayCount >= 10)
            {
                ModelState.AddModelError("", "You have reached the daily limit of 10 quizzes. Try again tomorrow.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("", "Title is required");
                return View();
            }
            title = title.Trim();
            if (title.Length > 100)
            {
                ModelState.AddModelError("", "Title cannot exceed 100 characters.");
                return View();
            }
            totalQuestions = Math.Clamp(totalQuestions, 3, 15);
            var result = await _documentTextExtractor.ExtractAsync(file);

            if (result.Success)
            {
                var generation = await _quizGenerator.GenerateAsync(
                     result.Text, totalQuestions, HttpContext.RequestAborted);
                ViewBag.Generation = generation;

                if (generation.Success && generation.Quiz != null)
                {
                    var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
                    var filename = Path.GetFileName(file.FileName);
                    if (filename.Length > 255)
                    {
                        filename = filename.Substring(0, 255);
                    }
                    var quiz = new Quiz
                    {
                        UserId = userId,
                        Title = title,
                        SourceFileName = filename,
                        SourceFileType = extension,
                        TotalQuestions = generation.Quiz.Questions.Count,
                        ShareCode = Guid.NewGuid().ToString("N").Substring(0, 12),
                        IsPublic = true
                    };

                    for (int i = 0; i < generation.Quiz.Questions.Count; i++)
                    {
                        var gq = generation.Quiz.Questions[i];
                        var question = new Question
                        {
                            Text = gq.Text,
                            Explanation = gq.Explanation,
                            Order = i + 1,
                        };
                        for (int j = 0; j < gq.Options.Count; j++)
                        {
                            var option = new Option
                            {
                                Text = gq.Options[j],
                                IsCorrect = (j == gq.CorrectIndex),
                                Order = j + 1,
                            };
                            question.Options.Add(option);
                        }
                        quiz.Questions.Add(question);
                    }
                    try
                    {
                        _quizContext.Quizzes.Add(quiz);
                        await _quizContext.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = quiz.Id });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "An error occurred while saving the quiz. Please try again.");
                        return View(result);
                    }
                }
            }
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var quiz = await _quizContext.Quizzes.Where(q => q.Id == id)
                .Include(qz => qz.Questions)
                .ThenInclude(qn => qn.Options)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (quiz == null || userId == null || userId != quiz?.UserId)
            {
                return NotFound();
            }
            return View(quiz);
        }
        [HttpGet]
        public async Task<IActionResult> MyQuizzes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var quizzes = await _quizContext.Quizzes
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.Id)
                .AsNoTracking()
                .ToListAsync();
            return View(quizzes);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var quiz = await _quizContext.Quizzes
                .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);
            if (quiz == null)
            {
                return NotFound();
            }
            _quizContext.Quizzes.Remove(quiz);
            await _quizContext.SaveChangesAsync();
            return RedirectToAction("MyQuizzes");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Take(string shareCode)
        {
            if (string.IsNullOrWhiteSpace(shareCode))
            {
                return NotFound();
            }
            var quiz = await _quizContext.Quizzes
                .Where(q => q.ShareCode == shareCode && q.IsPublic)
                .Include(qz => qz.Questions)
                .ThenInclude(qn => qn.Options)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (quiz == null)
            {
                return NotFound();
            }
            return View(quiz);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
      
        public async Task<IActionResult> Submit(string shareCode, Dictionary<int, int> answers)
        {
            answers ??= new Dictionary<int, int>();
            var quiz = await _quizContext.Quizzes
                .Where(q => q.ShareCode == shareCode && q.IsPublic)
                .Include(qz => qz.Questions)
                .ThenInclude(qn => qn.Options)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (quiz == null)
            {
                return NotFound();
            }

            int score = 0;
            foreach (var question in quiz.Questions)
            {
                if (!answers.TryGetValue(question.Id, out var selectedOptionId))
                {
                    continue;
                }
                var selectedOption = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                if (selectedOption != null && selectedOption.IsCorrect)
                {
                    score++;
                }
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attempt = new Attempt
            {
                QuizId = quiz.Id,
                UserId = userId,
                Score = score,
                TotalQuestions = quiz.Questions.Count,
                AttemptedAt = DateTime.UtcNow
            };

            _quizContext.Attempts.Add(attempt);
            await _quizContext.SaveChangesAsync(); 
            var result = new QuizResultViewModel
            {
                Quiz = quiz,
                Score = score,
                UserAnswers = answers
            };

            return View("Result", result);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult>DashBoard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var quizzes = await _quizContext.Quizzes
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.Id)
                .AsNoTracking()
                .ToListAsync();
            return View(quizzes);
        }
    }
}