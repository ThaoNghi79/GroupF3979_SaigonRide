using Microsoft.AspNetCore.Mvc;
using SaigonRide.Data;
using SaigonRide.Models;

namespace SaigonRide.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AuthController(ApplicationDbContext db) => _db = db;


        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                TempData["AuthError"] = "Invalid email or password.";
                TempData["Panel"] = "login";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Register(string FullName, string Email, string Password,
                                      string ConfirmPassword, string Role,
                                      string? PassportNumber)
        {
            if (Password != ConfirmPassword)
            {
                TempData["AuthError"] = "Passwords do not match.";
                TempData["Panel"] = "register";
                return RedirectToAction("Index");
            }
            if (_db.Users.Any(u => u.Email == Email))
            {
                TempData["AuthError"] = "This email is already registered.";
                TempData["Panel"] = "register";
                return RedirectToAction("Index");
            }
            var role = Enum.TryParse<UserRole>(Role, out var r) ? r : UserRole.LocalCommuter;
            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password),
                Role = role,
                PassportNumber = PassportNumber,
                IsPassportVerified = false,
                WalletBalance = 200000
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            TempData["AuthSuccess"] = "Account created! Please sign in.";
            TempData["Panel"] = "login";
            return RedirectToAction("Index");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        public IActionResult ForgotPassword() => View();


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string Email, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["AuthError"] = "Passwords do not match.";
                return RedirectToAction("ForgotPassword");
            }
            var user = _db.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                TempData["AuthError"] = "Email not found.";
                return RedirectToAction("ForgotPassword");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _db.SaveChanges();
            TempData["AuthSuccess"] = "Password reset successfully! Please sign in.";
            TempData["Panel"] = "login";
            return RedirectToAction("Index");
        }
    }
}