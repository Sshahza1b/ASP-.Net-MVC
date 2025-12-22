using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Email se user find karna (Identity username base login karta hai)
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Login Successful!" });
                }
            }
            return Json(new { success = false, message = "Email ya Password galat hai." });
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = email, Email = email };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Account banne ke baad foran login karwa dena
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Json(new { success = true, message = "Account created and logged in!" });
                }

                // Agar koi error aaye (e.g. password weak hai ya email already exists)
                var error = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
                return Json(new { success = false, message = error });
            }
            return Json(new { success = false, message = "Invalid data." });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // Isay use karein taake AJAX 400 error khatam ho jaye
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Json(new { success = true, message = "Logged out successfully!" });
        }
    }
}