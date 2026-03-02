using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using restaurant_pos_system.Models;
using System.Linq;
using System.Threading.Tasks;

namespace restaurant_pos_system.Controllers 
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // LOGIN - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN - POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find user by selected role
            var user = _userManager.Users
                .FirstOrDefault(u => u.RoleType == model.Role);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid role or PIN.");
                return View(model);
            }

            // Verify PIN against stored hashed value
            var result = _userManager.PasswordHasher
                .VerifyHashedPassword(user, user.PinHash, model.Pin);

            if (result == PasswordVerificationResult.Success)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect based on role
                return model.Role switch
                {
                    "Manager" => RedirectToAction("Dashboard", "Manager"),
                    "Waiter" => RedirectToAction("Dashboard", "Waiter"),
                    "Kitchen" => RedirectToAction("Dashboard", "Kitchen"),
                    _ => RedirectToAction("Login")
                };
            }

            ModelState.AddModelError("", "Invalid role or PIN.");
            return View(model);
        }

        // LOGOUT
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ACCESS DENIED
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}