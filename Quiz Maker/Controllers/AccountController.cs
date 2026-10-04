using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quiz_Maker.Models;
using Quiz_Maker.ViewModel;

namespace Quiz_Maker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {

            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if(User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DisplayName = model.DisplayName
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    if(error.Code== "DuplicateUserName")
                    {
                        continue;

                    }
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {

            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked out. Please try again after 15 minutes.");
                    return View(model);
                }
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
