using Microsoft.AspNetCore.Mvc;
using SaigonRide.Data;
using SaigonRide.Models;
using System.Net;
using System.Net.Mail;

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
            if (user.IsLocked)
            {
                TempData["AuthError"] = "Your account has been suspended. Please contact support.";
                TempData["Panel"] = "login";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("AvatarUrl", user.AvatarUrl ?? "");
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetInt32("AlertStation", user.AlertStationOverload ? 1 : 0);
            HttpContext.Session.SetInt32("AlertMaint", user.AlertVehicleMaintenance ? 1 : 0);
            HttpContext.Session.SetInt32("AlertInventory", user.AlertStationInventory ? 1 : 0);
            if ((int)user.Role == 2)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return RedirectToAction("Index", "UserRental");
            }
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
                WalletBalance = 0
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
        public IActionResult ResetPassword(string Email, string NewPassword, string ConfirmPassword, string VerificationCode)
        {
            string savedCode = HttpContext.Session.GetString("ResetCode") ?? "";
            if (string.IsNullOrEmpty(savedCode) || savedCode != VerificationCode)
            {
                TempData["AuthError"] = "Invalid or expired verification code!";
                return RedirectToAction("ForgotPassword");
            }
            HttpContext.Session.Remove("ResetCode");
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
            if (BCrypt.Net.BCrypt.Verify(NewPassword, user.PasswordHash))
            {
                TempData["AuthError"] = "New password must be different from the old password.";
                return RedirectToAction("ForgotPassword");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _db.SaveChanges();
            TempData["AuthSuccess"] = "Password reset successfully! Please sign in.";
            TempData["Panel"] = "login";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SendResetCode(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Please enter your email!" });

            if (!_db.Users.Any(u => u.Email == email))
                return Json(new { success = false, message = "Email not found in our system." });

            try
            {
                Random rnd = new Random();
                string code = rnd.Next(100000, 999999).ToString();
                HttpContext.Session.SetString("ResetCode", code);

                var fromAddress = new MailAddress("student399799@gmail.com", "SaigonRide");
                var toAddress = new MailAddress(email);
                const string fromPassword = "zzmd hsyv whus jygo";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "SaigonRide - Password Reset Code",
                    Body = $"Hello,\n\nYour password reset code is: {code}\n\nDo not share this code with anyone."
                })
                {
                    smtp.Send(message);
                }

                return Json(new { success = true, message = "Reset code sent to your email." });
            }
            catch
            {
                return Json(new { success = false, message = "Unable to send email." });
            }
        }
    }
}