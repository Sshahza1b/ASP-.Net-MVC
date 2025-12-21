using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Customer.Controllers
{
    [Area("Customer")] // Area define karna zaroori hai
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor Injection: Ye Identity services ko controller mein lata hai
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Email and Password are required." });
            }

            // Email se user find karna kyunki Identity login username mangta hai
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Login Successful! Redirecting..." });
                }
            }

            return Json(new { success = false, message = "Invalid email or password." });
        }
    }
}