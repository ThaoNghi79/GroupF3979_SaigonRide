using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Models;

namespace SaigonRide.Controllers
{
    public class RentalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int CurrentUserId = 2;

        public RentalController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Rent(int? stationId)
        {
            var selectedStationId = stationId ?? 1;
            ViewBag.Stations = await _context.Stations.OrderBy(s => s.StationName).ToListAsync();
            ViewBag.SelectedStationId = selectedStationId;
            ViewBag.SelectedStation = await _context.Stations.FirstOrDefaultAsync(s => s.StationId == selectedStationId);

            var vehicles = await _context.Vehicles
                .Include(v => v.Category)
                .Include(v => v.Station)
                .Where(v => v.StationId == selectedStationId)
                .OrderByDescending(v => v.Status == VehicleStatus.Available)
                .ThenBy(v => v.VehicleName)
                .ToListAsync();

            return View(vehicles);
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Category)
                .Include(v => v.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int vehicleId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle == null) return NotFound();
            if (vehicle.Status != VehicleStatus.Available) return RedirectToAction(nameof(Rent), new { stationId = vehicle.StationId });

            var activeRental = await _context.Rentals
                .AnyAsync(r => r.UserId == CurrentUserId && r.Status == RentalStatus.Active);

            if (activeRental) return RedirectToAction(nameof(MyTrip));

            var rental = new Rental
            {
                UserId = CurrentUserId,
                VehicleId = vehicle.VehicleId,
                StartStationId = vehicle.StationId,
                StartTime = DateTime.Now,
                Status = RentalStatus.Active
            };

            vehicle.Status = VehicleStatus.InTransit;
            if (vehicle.Station != null && vehicle.Station.CurrentInventory > 0)
            {
                vehicle.Station.CurrentInventory -= 1;
            }

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyTrip));
        }

        public async Task<IActionResult> MyTrip(int? selectedStationId)
        {
            var rental = await GetCurrentRental();
            var stations = await _context.Stations.OrderBy(s => s.CurrentInventory).ToListAsync();

            ViewBag.Stations = stations;
            ViewBag.SelectedStationId = selectedStationId;
            ViewBag.SelectedStation = selectedStationId.HasValue
                ? stations.FirstOrDefault(s => s.StationId == selectedStationId.Value)
                : null;

            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int rentalId, int destinationStationId)
        {
            var rental = await _context.Rentals
                .Include(r => r.Vehicle).ThenInclude(v => v!.Category)
                .Include(r => r.Vehicle).ThenInclude(v => v!.Station)
                .Include(r => r.StartStation)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId && r.UserId == CurrentUserId);

            var destinationStation = await _context.Stations.FindAsync(destinationStationId);

            if (rental == null || destinationStation == null) return NotFound();
            if (rental.Status != RentalStatus.Active) return RedirectToAction(nameof(Payment));

            var endTime = DateTime.Now;
            var duration = Math.Max(45, (int)Math.Ceiling((endTime - rental.StartTime).TotalMinutes));
            var rate = rental.Vehicle?.Category?.PricePerMinute ?? 0;
            var baseFare = duration * rate;
            var isLowInventory = destinationStation.Capacity > 0 && ((decimal)destinationStation.CurrentInventory / destinationStation.Capacity) < 0.2m;
            var discount = isLowInventory ? Math.Round(baseFare * 0.15m, 0) : 0m;
            var finalFare = baseFare - discount;

            rental.DestinationStationId = destinationStation.StationId;
            rental.EndTime = endTime;
            rental.DurationMinutes = duration;
            rental.BaseFare = baseFare;
            rental.DiscountAmount = discount;
            rental.FinalFare = finalFare;
            rental.Status = RentalStatus.PendingPayment;

            if (rental.Vehicle != null)
            {
                rental.Vehicle.Status = VehicleStatus.Available;
                rental.Vehicle.StationId = destinationStation.StationId;
            }

            destinationStation.CurrentInventory += 1;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Payment));
        }

        public async Task<IActionResult> Payment(string tab = "domestic", PaymentMethod method = PaymentMethod.MoMo)
        {
            var rental = await GetPendingPaymentRental();
            if (rental == null) return RedirectToAction(nameof(Rent));

            ViewBag.Tab = tab;
            ViewBag.Method = method;
            return View(rental);
        }

        private async Task<Rental?> GetCurrentRental()
        {
            return await _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle).ThenInclude(v => v!.Category)
                .Include(r => r.StartStation)
                .Include(r => r.DestinationStation)
                .Where(r => r.UserId == CurrentUserId && r.Status == RentalStatus.Active)
                .OrderByDescending(r => r.StartTime)
                .FirstOrDefaultAsync();
        }

        private async Task<Rental?> GetPendingPaymentRental()
        {
            return await _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle).ThenInclude(v => v!.Category)
                .Include(r => r.StartStation)
                .Include(r => r.DestinationStation)
                .Where(r => r.UserId == CurrentUserId && r.Status == RentalStatus.PendingPayment)
                .OrderByDescending(r => r.StartTime)
                .FirstOrDefaultAsync();
        }
    }
}
