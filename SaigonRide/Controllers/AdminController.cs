using Microsoft.AspNetCore.Mvc;
using SaigonRide.Data;
using SaigonRide.Models.ViewModels;

namespace SaigonRide.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _db.Users.Find(userId);
            if (user == null) return RedirectToAction("Login", "Auth");

            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();

            var vm = new AdminProfileViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                AlertStationOverload = user.AlertStationOverload,
                AlertVehicleMaintenance = user.AlertVehicleMaintenance,
                AlertStationInventory = user.AlertStationInventory,
            };

            return View("~/Views/Dashboard/Admin_Profile.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAdminProfile(AdminProfileViewModel vm, IFormFile? AvatarFile)
        {
            var user = _db.Users.Find(vm.UserId);
            if (user == null) return RedirectToAction("Login", "Auth");

            user.FullName = vm.FullName;

            if (!string.IsNullOrEmpty(vm.Email))
                user.Email = vm.Email;

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(folder);
                var fileName = $"avatar_{vm.UserId}_{Guid.NewGuid()}{Path.GetExtension(AvatarFile.FileName)}";
                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }
                user.AvatarUrl = $"/uploads/avatars/{fileName}";
                HttpContext.Session.SetString("AvatarUrl", user.AvatarUrl);
            }

            _db.SaveChanges();
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAlertSettings(AdminProfileViewModel vm)
        {
            var user = _db.Users.Find(vm.UserId);
            if (user == null) return RedirectToAction("Login", "Auth");

            user.AlertStationOverload = vm.AlertStationOverload;
            user.AlertVehicleMaintenance = vm.AlertVehicleMaintenance;
            user.AlertStationInventory = vm.AlertStationInventory;
            _db.SaveChanges();

            HttpContext.Session.SetInt32("AlertStation", vm.AlertStationOverload ? 1 : 0);
            HttpContext.Session.SetInt32("AlertMaint", vm.AlertVehicleMaintenance ? 1 : 0);
            HttpContext.Session.SetInt32("AlertInventory", vm.AlertStationInventory ? 1 : 0);

            TempData["SuccessMessage"] = "Notification settings saved.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAdminPassword(AdminProfileViewModel vm)
        {
            var user = _db.Users.Find(vm.UserId);
            if (user == null) return RedirectToAction("Login", "Auth");

            if (!BCrypt.Net.BCrypt.Verify(vm.CurrentPassword, user.PasswordHash))
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("Profile");
            }

            if (vm.NewPassword != vm.ConfirmNewPassword)
            {
                TempData["ErrorMessage"] = "New passwords do not match.";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }
    }
}