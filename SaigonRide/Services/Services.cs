using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Models;

namespace SaigonRide.Services
{

    public interface IInventoryService
    {
        (int current, int max) GetStationInventory(int stationId);
        void UpdateStationInventory(int stationId, int delta);
    }

    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _db;
        public InventoryService(ApplicationDbContext db) => _db = db;

        public (int current, int max) GetStationInventory(int stationId)
        {
            var s = _db.Stations.Find(stationId);
            return s == null ? (0, 0) : (s.CurrentInventory, s.Capacity);
        }

        public void UpdateStationInventory(int stationId, int delta)
        {
            var s = _db.Stations.Find(stationId);
            if (s == null) return;
            s.CurrentInventory = Math.Max(0, s.CurrentInventory + delta);
            _db.SaveChanges();
        }
    }


    public interface IVehicleService
    {
        IEnumerable<Vehicle> GetVehicles(string? category, string? status, int? stationId);
        Vehicle? GetById(int id);
        void Create(Vehicle v);
        void Update(int id, Vehicle data);
        void Delete(int id);
    }

    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _db;
        private readonly IInventoryService _inv;
        public VehicleService(ApplicationDbContext db, IInventoryService inv)
        { _db = db; _inv = inv; }

        public IEnumerable<Vehicle> GetVehicles(string? category, string? status, int? stationId)
        {
            var q = _db.Vehicles.Include(v => v.Category).Include(v => v.Station).AsQueryable();
            if (!string.IsNullOrEmpty(category))
                q = q.Where(v => v.Category!.CategoryName == category);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<VehicleStatus>(status, out var s))
                q = q.Where(v => v.Status == s);
            if (stationId.HasValue)
                q = q.Where(v => v.StationId == stationId);
            return q.OrderBy(v => v.VehicleCode).ToList();
        }

        public Vehicle? GetById(int id) =>
            _db.Vehicles.Include(v => v.Category).Include(v => v.Station)
               .FirstOrDefault(v => v.VehicleId == id);

        public void Create(Vehicle v)
        {
            var (cur, max) = _inv.GetStationInventory(v.StationId);
            if (cur >= max) throw new InvalidOperationException("Station is full. Cannot add vehicle.");
            v.Status = VehicleStatus.Available;
            _db.Vehicles.Add(v);
            _db.SaveChanges();
            _inv.UpdateStationInventory(v.StationId, +1);
        }

        public void Update(int id, Vehicle data)
        {
            var existing = _db.Vehicles.Find(id)
                ?? throw new KeyNotFoundException("Vehicle not found.");

            int oldStation = existing.StationId;
            existing.VehicleCode = data.VehicleCode;
            existing.VehicleName = data.VehicleName;
            existing.CategoryId = data.CategoryId;


            if (data.StationId != oldStation)
            {
                var (cur, max) = _inv.GetStationInventory(data.StationId);
                if (cur >= max) throw new InvalidOperationException("New station is full.");
                _inv.UpdateStationInventory(oldStation, -1);
                existing.StationId = data.StationId;
                _inv.UpdateStationInventory(data.StationId, +1);
            }

            existing.Status = data.Status;
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var v = _db.Vehicles.Find(id)
                ?? throw new KeyNotFoundException("Vehicle not found.");
            if (v.Status == VehicleStatus.InTransit)
                throw new InvalidOperationException("Cannot delete vehicle in active rental.");
            int sid = v.StationId;
            _db.Vehicles.Remove(v);
            _db.SaveChanges();
            _inv.UpdateStationInventory(sid, -1);
        }
    }

    public interface IStationService
    {
        IEnumerable<Station> GetAll();
        Station? GetById(int id);
        void Create(Station s);
        void Update(int id, Station data);
        void Delete(int id);
    }

    public class StationService : IStationService
    {
        private readonly ApplicationDbContext _db;
        public StationService(ApplicationDbContext db) => _db = db;

        public IEnumerable<Station> GetAll() =>
            _db.Stations.OrderBy(s => s.StationName).ToList();

        public Station? GetById(int id) => _db.Stations.Find(id);

        public void Create(Station s)
        {
            s.CurrentInventory = 0;
            _db.Stations.Add(s);
            _db.SaveChanges();
        }

        public void Update(int id, Station data)
        {
            var existing = _db.Stations.Find(id)
                ?? throw new KeyNotFoundException("Station not found.");
            existing.StationName = data.StationName;
            existing.Location = data.Location;
            existing.Capacity = data.Capacity;
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var s = _db.Stations.Find(id)
                ?? throw new KeyNotFoundException("Station not found.");
            bool hasVehicles = _db.Vehicles.Any(v => v.StationId == id);
            if (hasVehicles)
                throw new InvalidOperationException("Cannot delete station with vehicles assigned.");
            _db.Stations.Remove(s);
            _db.SaveChanges();
        }
    }

    public interface IReportService
    {
        IEnumerable<StationInventoryReportItem> GetStationInventoryReport();
    }

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        public ReportService(ApplicationDbContext db) => _db = db;

        public IEnumerable<StationInventoryReportItem> GetStationInventoryReport()
        {
            return _db.Stations
                .Select(s => new StationInventoryReportItem
                {
                    StationId = s.StationId,
                    StationName = s.StationName,
                    Location = s.Location,
                    Capacity = s.Capacity,
                    CurrentInventory = s.CurrentInventory,
                    BikeCount = _db.Vehicles.Count(v => v.StationId == s.StationId && v.Category!.CategoryName == "Standard Bike"),
                    ScooterCount = _db.Vehicles.Count(v => v.StationId == s.StationId && v.Category!.CategoryName == "E-Scooter"),
                    UtilizationPercent = s.Capacity > 0
                        ? Math.Round((double)s.CurrentInventory / s.Capacity * 100, 1) : 0
                })
                .OrderByDescending(x => x.UtilizationPercent)
                .ToList();
        }
    }

    public class StationInventoryReportItem
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = "";
        public string Location { get; set; } = "";
        public int Capacity { get; set; }
        public int CurrentInventory { get; set; }
        public int BikeCount { get; set; }
        public int ScooterCount { get; set; }
        public double UtilizationPercent { get; set; }
        public int AvailableSlots => Capacity - CurrentInventory;
        public bool IsLowInventory => UtilizationPercent < 20;
        public bool IsOverloaded => UtilizationPercent >= 90;
        public bool IsBalanced => !IsLowInventory && !IsOverloaded;
    }

    public interface IRevenueReportService
    {
        RevenueReportData GetRevenueReport(DateTime? from = null, DateTime? to = null);
    }

    public class RevenueReportService : IRevenueReportService
    {
        private readonly ApplicationDbContext _db;
        public RevenueReportService(ApplicationDbContext db) => _db = db;

        public RevenueReportData GetRevenueReport(DateTime? from = null, DateTime? to = null)
        {
            var query = _db.Rentals
                .Include(r => r.Vehicle).ThenInclude(v => v!.Category)
                .Include(r => r.Payment)
                .Where(r => r.Status == RentalStatus.Completed)
                .AsQueryable();

            if (from.HasValue) query = query.Where(r => r.StartTime >= from.Value);
            if (to.HasValue) query = query.Where(r => r.StartTime <= to.Value.AddDays(1));

            var rentals = query.ToList();


            var byCategory = rentals
                .GroupBy(r => r.Vehicle?.Category?.CategoryName ?? "Unknown")
                .Select(g => new RevenueByCategoryItem
                {
                    CategoryName = g.Key,
                    TotalRentals = g.Count(),
                    TotalRevenue = g.Sum(r => r.FinalFare),
                    TotalDiscount = g.Sum(r => r.DiscountAmount),
                    AvgDuration = g.Any() ? (int)g.Average(r => r.DurationMinutes) : 0
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();


            var byDay = rentals
                .GroupBy(r => r.StartTime.Date)
                .Select(g => new RevenueByDayItem
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(r => r.FinalFare),
                    BikeRevenue = g.Where(r => r.Vehicle?.Category?.CategoryName == "Standard Bike").Sum(r => r.FinalFare),
                    ScooterRevenue = g.Where(r => r.Vehicle?.Category?.CategoryName == "E-Scooter").Sum(r => r.FinalFare),
                    RentalCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return new RevenueReportData
            {
                TotalRevenue = rentals.Sum(r => r.FinalFare),
                TotalRentals = rentals.Count,
                TotalDiscount = rentals.Sum(r => r.DiscountAmount),
                DiscountedRentals = rentals.Count(r => r.DiscountAmount > 0),
                ByCategory = byCategory,
                ByDay = byDay
            };
        }
    }

    public class RevenueReportData
    {
        public decimal TotalRevenue { get; set; }
        public int TotalRentals { get; set; }
        public decimal TotalDiscount { get; set; }
        public int DiscountedRentals { get; set; }
        public List<RevenueByCategoryItem> ByCategory { get; set; } = new();
        public List<RevenueByDayItem> ByDay { get; set; } = new();
    }

    public class RevenueByCategoryItem
    {
        public string CategoryName { get; set; } = "";
        public int TotalRentals { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public int AvgDuration { get; set; }
    }

    public class RevenueByDayItem
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal BikeRevenue { get; set; }
        public decimal ScooterRevenue { get; set; }
        public int RentalCount { get; set; }
    }
}