using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Models;
using SaigonRide.Services;

namespace SaigonRide.Controllers
{
    // ═══════════════════════════════════════════════════
    //  DASHBOARD CONTROLLER
    // ═══════════════════════════════════════════════════
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db) => _db = db;

        // Helper: kiểm tra đăng nhập
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
    }

    // ═══════════════════════════════════════════════════
    //  VEHICLE CONTROLLER  (UC1)
    // ═══════════════════════════════════════════════════
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleSvc;
        private readonly ApplicationDbContext _db;

        public VehicleController(IVehicleService vehicleSvc, ApplicationDbContext db)
        { _vehicleSvc = vehicleSvc; _db = db; }

        // READ — List với filter
        public IActionResult Index(string? category, string? status, int? stationId)
        {
            var vehicles = _vehicleSvc.GetVehicles(category, status, stationId);
            LoadDropdowns();
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(vehicles);
        }

        // CREATE — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _vehicleSvc.Create(vehicle);
                TempData["Success"] = $"Vehicle '{vehicle.VehicleName}' created successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // EDIT — GET
        public IActionResult Edit(int id)
        {
            var vehicle = _vehicleSvc.GetById(id);
            if (vehicle == null) return NotFound();
            LoadDropdownsForEdit(vehicle);
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(vehicle);
        }

        // EDIT — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownsForEdit(vehicle);
                return View(vehicle);
            }
            try
            {
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

        // DELETE — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _vehicleSvc.Delete(id);
                TempData["Success"] = "Vehicle deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ──
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

        // POST /Vehicle/UpdateStatus/{id} — bulk update status
        [HttpPost]
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateRequest req)
        {
            var v = _db.Vehicles.Find(id);
            if (v == null) return NotFound();
            if (Enum.TryParse<VehicleStatus>(req.Status, out var s))
            {
                v.Status = s;
                _db.SaveChanges();
            }
            return Ok();
        }
    }

    public class StatusUpdateRequest { public string Status { get; set; } = ""; }

    // ═══════════════════════════════════════════════════
    //  STATION CONTROLLER  (UC1)
    // ═══════════════════════════════════════════════════
    public class StationController : Controller
    {
        private readonly IStationService _stationSvc;
        private readonly ApplicationDbContext _db;

        public StationController(IStationService stationSvc, ApplicationDbContext db)
        {
            _stationSvc = stationSvc;
            _db = db;
        }

        // READ
        public IActionResult Index()
        {
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(_stationSvc.GetAll());
        }

        // CREATE — POST
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
                // Gán Status mặc định nếu chưa có
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

        // EDIT — GET
        public IActionResult Edit(int id)
        {
            var station = _stationSvc.GetById(id);
            if (station == null) return NotFound();
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(station);
        }

        // EDIT — POST (ĐÃ FIX BẰNG TIẾNG ANH & SỬA LỖI IENUMERABLE)
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

            // Update các trường cơ bản
            stationFromDb.StationName = obj.StationName;
            stationFromDb.Location = obj.Location;
            stationFromDb.Capacity = obj.Capacity;

            // Update 3 trường mới cho UI/UX
            stationFromDb.Status = obj.Status;
            if (obj.Latitude != null) stationFromDb.Latitude = obj.Latitude;
            if (obj.Longitude != null) stationFromDb.Longitude = obj.Longitude;

            _db.Stations.Update(stationFromDb);
            _db.SaveChanges();

            TempData["Success"] = $"Station '{obj.StationName}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // BULK UPDATE STATUS (ĐÃ FIX BẰNG TIẾNG ANH)
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

        // DELETE — POST
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

// ═══════════════════════════════════════════════════
//  REPORT CONTROLLER
// ═══════════════════════════════════════════════════
public class ReportController : Controller
    {
        private readonly IReportService _reportSvc;
        private readonly IRevenueReportService _revenueSvc;
        private readonly ApplicationDbContext _db;

        public ReportController(IReportService reportSvc,
                                IRevenueReportService revenueSvc,
                                ApplicationDbContext db)
        { _reportSvc = reportSvc; _revenueSvc = revenueSvc; _db = db; }

        // UC1 — Station Inventory Report (Thao Nghi)
        public IActionResult StationInventory()
        {
            ViewBag.VehicleCount = _db.Vehicles.Count();
            ViewBag.StationCount = _db.Stations.Count();
            return View(_reportSvc.GetStationInventoryReport());
        }

        // UC2 — Revenue Report (Nhu Quynh)
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
}