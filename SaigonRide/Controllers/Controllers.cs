using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Models;
using SaigonRide.Services;

namespace SaigonRide.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db) => _db = db;

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            ViewBag.TotalVehicles = _db.Vehicles.Count();
            ViewBag.ActiveRentals = _db.Rentals.Count(r => r.Status == RentalStatus.Active);
            ViewBag.LowInventoryStations = _db.Stations.Count(s => s.Capacity > 0 && (double)s.CurrentInventory / s.Capacity < 0.20);
            ViewBag.OverloadedStations = _db.Stations.Count(s => s.Capacity > 0 && (double)s.CurrentInventory / s.Capacity >= 0.90);
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            ViewBag.TotalRevenue = _db.Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => (decimal?)p.Amount) ?? 0;

            var recentRentals = _db.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle).ThenInclude(v => v!.Category)
                .Include(r => r.StartStation)
                .Include(r => r.DestinationStation)
                .OrderByDescending(r => r.StartTime)
                .Take(5)
                .ToList();

            ViewBag.StationMapData = _db.Stations.ToList();
            return View(recentRentals);
        }

        public IActionResult Map()
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            ViewBag.LowInventoryStations = _db.Stations.Count(s => s.Capacity > 0 && (double)s.CurrentInventory / s.Capacity < 0.20);
            ViewBag.OverloadedStations = _db.Stations.Count(s => s.Capacity > 0 && (double)s.CurrentInventory / s.Capacity >= 0.90);
            ViewBag.StationMapData = _db.Stations.ToList();
            ViewBag.RentalsMapData = _db.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle).ThenInclude(v => v!.Category)
                .Where(r => r.Status == RentalStatus.Active || r.Status == RentalStatus.Completed)
                .OrderByDescending(r => r.StartTime).Take(20).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 1)
                return Json(new { vehicles = Array.Empty<object>(), stations = Array.Empty<object>() });

            var keyword = q.Trim().ToLower();

            var vehicles = _db.Vehicles
                .Include(v => v.Category)
                .Where(v => v.VehicleName.ToLower().Contains(keyword)
                         || v.VehicleCode.ToLower().Contains(keyword))
                .Select(v => new {
                    v.VehicleId,
                    v.VehicleName,
                    v.VehicleCode,
                    CategoryName = v.Category != null ? v.Category.CategoryName : ""
                })
                .Take(5)
                .ToList();

            var stations = _db.Stations
                .Where(s => s.StationName.ToLower().Contains(keyword)
                         || s.Location.ToLower().Contains(keyword))
                .Select(s => new {
                    s.StationId,
                    s.StationName,
                    s.Location
                })
                .Take(5)
                .ToList();

            return Json(new { vehicles, stations });
        }
    }

    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleSvc;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public VehicleController(IVehicleService vehicleSvc, ApplicationDbContext db, IWebHostEnvironment env)
        { _vehicleSvc = vehicleSvc; _db = db; _env = env; }

        public IActionResult Index(string? category, string? status, int? stationId)
        {
            var vehicles = _vehicleSvc.GetVehicles(category, status, stationId);
            LoadDropdowns();
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(vehicles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                    vehicle.ImageUrl = await SaveVehicleImage(ImageFile);
                _vehicleSvc.Create(vehicle);
                TempData["Success"] = $"Vehicle '{vehicle.VehicleName}' created successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var vehicle = _vehicleSvc.GetById(id);
            if (vehicle == null) return NotFound();
            LoadDropdownsForEdit(vehicle);
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle, IFormFile? ImageFile, bool RemoveImage = false)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownsForEdit(vehicle);
                return View(vehicle);
            }
            try
            {
                var existing = _db.Vehicles.AsNoTracking().FirstOrDefault(v => v.VehicleId == id);

                if (ImageFile != null && ImageFile.Length > 0)
                {

                    if (!string.IsNullOrEmpty(existing?.ImageUrl))
                        DeleteVehicleImage(existing.ImageUrl);
                    vehicle.ImageUrl = await SaveVehicleImage(ImageFile);
                }
                else if (RemoveImage)
                {
                    if (!string.IsNullOrEmpty(existing?.ImageUrl))
                        DeleteVehicleImage(existing.ImageUrl);
                    vehicle.ImageUrl = null;
                }
                else
                {
                    vehicle.ImageUrl = existing?.ImageUrl;
                }

                _vehicleSvc.Update(id, vehicle);
                TempData["Success"] = $"Vehicle '{vehicle.VehicleName}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                LoadDropdownsForEdit(vehicle);
                return View(vehicle);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var vehicle = _db.Vehicles.Find(id);
                if (!string.IsNullOrEmpty(vehicle?.ImageUrl))
                    DeleteVehicleImage(vehicle.ImageUrl);
                _vehicleSvc.Delete(id);
                TempData["Success"] = "Vehicle deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns()
        {
            ViewBag.Categories = _db.VehicleCategories.ToList();
            ViewBag.Stations = _db.Stations.ToList();
        }

        private void LoadDropdownsForEdit(Vehicle v)
        {
            ViewBag.Categories = new SelectList(_db.VehicleCategories, "CategoryId", "CategoryName", v.CategoryId);
            ViewBag.Stations = new SelectList(_db.Stations.Select(s => new {
                s.StationId,
                Display = $"{s.StationName} ({s.CurrentInventory}/{s.Capacity})"
            }), "StationId", "Display", v.StationId);
            ViewBag.Statuses = new SelectList(
                Enum.GetValues<VehicleStatus>().Select(e => new { Value = e.ToString(), Text = e.ToString() }),
                "Value", "Text", v.Status.ToString()
            );
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateRequest req)
        {
            var v = _db.Vehicles.Find(id);
            if (v == null) return NotFound();

            if (Enum.TryParse<VehicleStatus>(req.Status, out var s))
            {
                v.Status = s;
                _db.SaveChanges();

                if (s == VehicleStatus.Maintenance)
                {
                    var adminEmail = HttpContext.Session.GetString("UserEmail");
                    var alertOn = HttpContext.Session.GetInt32("AlertMaint") == 1;

                    if (alertOn && !string.IsNullOrEmpty(adminEmail))
                    {
                        new EmailService().Send(
                            adminEmail,
                            "[SaigonRide] Vehicle Maintenance Alert",
                            $"Vehicle {v.VehicleName} ({v.VehicleCode}) has been set to Maintenance status."
                        );
                    }
                }
            }

            return Ok();
        }
        private async Task<string> SaveVehicleImage(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new InvalidOperationException("Only JPG, PNG, WEBP, GIF images are allowed.");
            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("Image size must be under 5MB.");

            var folder = Path.Combine(_env.WebRootPath, "uploads", "vehicles");
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"vehicle_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/vehicles/{fileName}";
        }

        private void DeleteVehicleImage(string imageUrl)
        {
            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch { }
        }
    }

    public class StatusUpdateRequest { public string Status { get; set; } = ""; }

    public class StationController : Controller
    {
        private readonly IStationService _stationSvc;
        private readonly ApplicationDbContext _db;

        public StationController(IStationService stationSvc, ApplicationDbContext db)
        {
            _stationSvc = stationSvc;
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            var stations = _db.Stations
                .Include(s => s.Vehicles)
                    .ThenInclude(v => v.Category)
                .ToList();
            return View(stations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Station station)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                if (string.IsNullOrEmpty(station.Status))
                {
                    station.Status = "Active";
                }

                _stationSvc.Create(station);
                TempData["Success"] = $"Station '{station.StationName}' created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var station = _stationSvc.GetById(id);
            if (station == null) return NotFound();
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(station);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Station obj)
        {
            var stationFromDb = _db.Stations.Find(id);
            if (stationFromDb == null)
            {
                TempData["Error"] = "Station not found!";
                return RedirectToAction(nameof(Index));
            }

            stationFromDb.StationName = obj.StationName;
            stationFromDb.Location = obj.Location;
            stationFromDb.Capacity = obj.Capacity;

            stationFromDb.Status = obj.Status;
            if (obj.Latitude != null) stationFromDb.Latitude = obj.Latitude;
            if (obj.Longitude != null) stationFromDb.Longitude = obj.Longitude;

            _db.Stations.Update(stationFromDb);
            _db.SaveChanges();

            var adminEmail = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(adminEmail))
            {
                try
                {
                    var emailSvc = new EmailService();

                    if (HttpContext.Session.GetInt32("AlertStation") == 1)
                    {
                        if (stationFromDb.CurrentInventory == 0)
                        {
                            emailSvc.Send(adminEmail,
                                "[SaigonRide] Station Empty Alert",
                                $"Station '{stationFromDb.StationName}' is now empty (0 vehicles).");
                        }
                        else if (stationFromDb.Capacity > 0 && (double)stationFromDb.CurrentInventory / stationFromDb.Capacity >= 0.80)
                        {
                            emailSvc.Send(adminEmail,
                                "[SaigonRide] Station Overload Alert",
                                $"Station '{stationFromDb.StationName}' is at {(int)((double)stationFromDb.CurrentInventory / stationFromDb.Capacity * 100)}% capacity.");
                        }
                    }

                    if (HttpContext.Session.GetInt32("AlertInventory") == 1 && stationFromDb.IsLowInventory())
                    {
                        emailSvc.Send(adminEmail,
                            "[SaigonRide] Low Inventory Alert",
                            $"Station '{stationFromDb.StationName}' is below 20% capacity ({stationFromDb.CurrentInventory}/{stationFromDb.Capacity}).");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Email error: {ex.Message}";
                }
            }

            TempData["Success"] = $"Station '{obj.StationName}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BulkUpdateStatus(List<int> stationIds, string NewStatus)
        {
            if (stationIds == null || !stationIds.Any())
            {
                TempData["Error"] = "No stations selected for update.";
                return RedirectToAction(nameof(Index));
            }

            var stationsToUpdate = _db.Stations.Where(s => stationIds.Contains(s.StationId)).ToList();

            foreach (var station in stationsToUpdate)
            {
                station.Status = NewStatus;
            }

            _db.SaveChanges();

            TempData["Success"] = $"Successfully updated {stationsToUpdate.Count} stations to '{NewStatus}'!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _stationSvc.Delete(id);
                TempData["Success"] = "Station deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }

    public class ReportController : Controller
    {
        private readonly IReportService _reportSvc;
        private readonly IRevenueReportService _revenueSvc;
        private readonly ApplicationDbContext _db;

        public ReportController(IReportService reportSvc,
                                IRevenueReportService revenueSvc,
                                ApplicationDbContext db)
        { _reportSvc = reportSvc; _revenueSvc = revenueSvc; _db = db; }

        public IActionResult StationInventory()
        {
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(_reportSvc.GetStationInventoryReport());
        }

        public IActionResult Revenue(DateTime? from, DateTime? to)
        {
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            ViewBag.From = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.To = to?.ToString("yyyy-MM-dd") ?? "";
            var data = _revenueSvc.GetRevenueReport(from, to);
            return View(data);
        }
    }
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserManagementController(ApplicationDbContext db) => _db = db;

        public IActionResult Index()
        {
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            ViewBag.TotalLocal = _db.Users.Count(u => u.Role == UserRole.LocalCommuter);
            ViewBag.TotalTourist = _db.Users.Count(u => u.Role == UserRole.ForeignTourist);

            var users = _db.Users
                .Where(u => u.Role != UserRole.Admin)
                .OrderBy(u => u.FullName)
                .ToList();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LockUser(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null || user.Role == UserRole.Admin)
                return NotFound();

            user.IsLocked = true;
            _db.SaveChanges();
            TempData["Success"] = $"Account '{user.FullName}' has been locked.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnlockUser(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            user.IsLocked = false;
            _db.SaveChanges();
            TempData["Success"] = $"Account '{user.FullName}' has been unlocked.";
            return RedirectToAction(nameof(Index));
        }
    }
}