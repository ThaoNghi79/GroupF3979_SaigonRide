using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Models;

namespace SaigonRide.Controllers
{
    public class UserRentalController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserRentalController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

        private bool IsLoggedIn()
        {
            return CurrentUserId != null;
        }

        private User? GetCurrentUser()
        {
            if (!CurrentUserId.HasValue) return null;

            return _db.Users.FirstOrDefault(u => u.UserId == CurrentUserId.Value);
        }

        public IActionResult Profile()
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var user = GetCurrentUser();

            if (user == null) return NotFound();

            ViewBag.CurrentUser = user;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TopUpWallet(decimal amount)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var user = GetCurrentUser();

            if (user == null) return NotFound();

            if (user.Role == UserRole.ForeignTourist)
            {
                TempData["Error"] = "Ví chỉ áp dụng cho người dùng nội địa.";
                return RedirectToAction(nameof(Profile));
            }

            if (amount <= 0)
            {
                TempData["Error"] = "Số tiền nạp không hợp lệ.";
                return RedirectToAction(nameof(Profile));
            }

            user.WalletBalance += amount;

            _db.SaveChanges();

            TempData["Success"] = "Nạp tiền vào ví thành công.";

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(
            string fullName,
            IFormFile? avatarFile,
            string? passportNumber)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Index", "Auth");

            var user = GetCurrentUser();

            if (user == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                TempData["Error"] = "Họ tên không hợp lệ.";
                return RedirectToAction(nameof(Profile));
            }

            user.FullName = fullName.Trim();

            if (user.Role == UserRole.ForeignTourist)
            {
                user.PassportNumber = passportNumber;
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "avatars");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(avatarFile.FileName);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    avatarFile.CopyTo(stream);
                }

                user.AvatarUrl = "/uploads/avatars/" + fileName;
            }

            _db.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công.";

            return RedirectToAction(nameof(Profile));
        }

        public IActionResult Index(int? stationId, int? categoryId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var stations = _db.Stations.ToList();
            var categories = _db.VehicleCategories.ToList();

            var vehicles = _db.Vehicles
                .Include(v => v.Category)
                .Include(v => v.Station)
                .AsQueryable();

            if (stationId.HasValue)
            {
                vehicles = vehicles.Where(v => v.StationId == stationId.Value);
            }

            if (categoryId.HasValue)
            {
                vehicles = vehicles.Where(v => v.CategoryId == categoryId.Value);
            }

            ViewBag.Stations = stations;
            ViewBag.Categories = categories;
            ViewBag.SelectedStation = stationId;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.CurrentUser = GetCurrentUser();

            return View(vehicles.ToList());
        }

        public IActionResult Confirm(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var vehicle = _db.Vehicles
                .Include(v => v.Category)
                .Include(v => v.Station)
                .FirstOrDefault(v => v.VehicleId == id);

            if (vehicle == null) return NotFound();

            if (vehicle.Status != VehicleStatus.Available)
            {
                TempData["Error"] = "This vehicle is not available for rental.";
                return RedirectToAction(nameof(Index), new { stationId = vehicle.StationId });
            }

            var stations = _db.Stations.ToList();
            var categories = _db.VehicleCategories.ToList();

            var vehicles = _db.Vehicles
                .Include(v => v.Category)
                .Include(v => v.Station)
                .ToList();

            ViewBag.Stations = stations;
            ViewBag.Categories = categories;
            ViewBag.Vehicles = vehicles;
            ViewBag.SelectedVehicle = vehicle;
            ViewBag.CurrentUser = GetCurrentUser();

            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartRental(int vehicleId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var userId = CurrentUserId!.Value;

            var user = GetCurrentUser();

            if (user == null) return RedirectToAction("Index", "Auth");

            var activeRental = _db.Rentals
                .FirstOrDefault(r => r.UserId == userId && r.Status == RentalStatus.Active);

            if (activeRental != null)
            {
                return RedirectToAction(nameof(MyTrip), new { id = activeRental.RentalId });
            }

            var vehicle = _db.Vehicles
                .Include(v => v.Station)
                .FirstOrDefault(v => v.VehicleId == vehicleId);

            if (vehicle == null) return NotFound();

            if (vehicle.Status != VehicleStatus.Available)
            {
                TempData["Error"] = "This vehicle is not available.";
                return RedirectToAction(nameof(Index), new { stationId = vehicle.StationId });
            }

            var startStation = vehicle.Station;

            if (startStation == null)
            {
                TempData["Error"] = "Start station not found.";
                return RedirectToAction(nameof(Index));
            }

            var rental = new Rental
            {
                UserId = userId,
                VehicleId = vehicle.VehicleId,
                StartStationId = startStation.StationId,
                StartTime = DateTime.Now,
                Status = RentalStatus.Active
            };

            vehicle.Status = VehicleStatus.InTransit;

            if (startStation.CurrentInventory > 0)
            {
                startStation.CurrentInventory -= 1;
            }

            _db.Rentals.Add(rental);

            _db.SaveChanges();

            return RedirectToAction(nameof(MyTrip), new { id = rental.RentalId });
        }

        public IActionResult StationMap()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Index", "Auth");

            var stations = _db.Stations
                .Include(s => s.Vehicles)
                    .ThenInclude(v => v.Category)
                .OrderBy(s => s.StationName)
                .ToList();

            ViewBag.CurrentUser = GetCurrentUser();

            return View(stations);
        }

        public IActionResult MyTrip(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var rental = _db.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v!.Category)
                .Include(r => r.StartStation)
                .Include(r => r.DestinationStation)
                .FirstOrDefault(r => r.RentalId == id && r.UserId == CurrentUserId);

            if (rental == null) return NotFound();

            var stations = _db.Stations
                .Include(s => s.Vehicles)
                    .ThenInclude(v => v.Category)
                .OrderBy(s => s.StationName)
                .ToList();

            ViewBag.Stations = stations;
            ViewBag.CurrentUser = GetCurrentUser();

            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectReturnStation(int rentalId, int destinationStationId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var rental = _db.Rentals
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v!.Category)
                .Include(r => r.StartStation)
                .FirstOrDefault(r => r.RentalId == rentalId && r.UserId == CurrentUserId);

            if (rental == null) return NotFound();

            if (rental.Status != RentalStatus.Active)
            {
                TempData["Error"] = "This rental is not active.";
                return RedirectToAction(nameof(MyTrip), new { id = rentalId });
            }

            var destinationStation = _db.Stations
                .FirstOrDefault(s => s.StationId == destinationStationId);

            if (destinationStation == null)
            {
                TempData["Error"] = "Destination station not found.";
                return RedirectToAction(nameof(MyTrip), new { id = rentalId });
            }

            if (destinationStation.CurrentInventory >= destinationStation.Capacity)
            {
                TempData["Error"] = "This station is full. Please choose another return station.";
                return RedirectToAction(nameof(MyTrip), new { id = rentalId });
            }

            var endTime = DateTime.Now;

            var totalSeconds = Math.Max(
                1,
                (decimal)(endTime - rental.StartTime).TotalSeconds);

            var exactMinutes = totalSeconds / 60m;

            var displayMinutes = (int)Math.Floor(exactMinutes);

            var pricePerMinute =
                rental.Vehicle?.Category?.PricePerMinute ?? 0m;

            var baseFare = exactMinutes * pricePerMinute;

            var roundedBaseFare = Math.Round(baseFare, 0);

            var isLowInventoryBeforeReturn =
                destinationStation.Capacity > 0 &&
                (double)destinationStation.CurrentInventory /
                destinationStation.Capacity < 0.20;

            var discount = isLowInventoryBeforeReturn
                ? roundedBaseFare * 0.15m
                : 0m;

            var finalFare = roundedBaseFare - discount;

            rental.DestinationStationId = destinationStation.StationId;
            rental.EndTime = endTime;
            rental.DurationMinutes = displayMinutes;
            rental.BaseFare = roundedBaseFare;
            rental.DiscountAmount = discount;
            rental.FinalFare = finalFare;
            rental.Status = RentalStatus.PendingPayment;

            if (rental.Vehicle != null)
            {
                rental.Vehicle.Status = VehicleStatus.Available;
                rental.Vehicle.StationId = destinationStation.StationId;
            }

            destinationStation.CurrentInventory += 1;

            _db.SaveChanges();

            return RedirectToAction(nameof(Checkout), new { id = rental.RentalId });
        }

        public IActionResult Checkout(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var rental = _db.Rentals
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v!.Category)
                .Include(r => r.StartStation)
                .Include(r => r.DestinationStation)
                .Include(r => r.Payment)
                .FirstOrDefault(r => r.RentalId == id && r.UserId == CurrentUserId);

            if (rental == null) return NotFound();

            ViewBag.CurrentUser = GetCurrentUser();

            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Pay(int rentalId, PaymentMethod method)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var rental = _db.Rentals
                .Include(r => r.User)
                .FirstOrDefault(r => r.RentalId == rentalId && r.UserId == CurrentUserId);

            if (rental == null) return NotFound();

            if (rental.Status != RentalStatus.PendingPayment)
            {
                TempData["Error"] = "This rental is not ready for payment.";
                return RedirectToAction(nameof(Checkout), new { id = rentalId });
            }

            if (!IsValidPaymentMethod(rental.User!.Role, method))
            {
                TempData["Error"] = "This payment method is not available for your user type.";
                return RedirectToAction(nameof(Checkout), new { id = rentalId });
            }

            if (method == PaymentMethod.Wallet)
            {
                if (rental.User.WalletBalance < rental.FinalFare)
                {
                    TempData["Error"] = "Số dư trong ví không đủ. Vui lòng chọn phương thức thanh toán khác.";
                    return RedirectToAction(nameof(Checkout), new { id = rentalId });
                }

                rental.User.WalletBalance -= rental.FinalFare;
            }

            var payment = new Payment
            {
                RentalId = rental.RentalId,
                UserId = rental.UserId,
                Method = method,
                Amount = rental.FinalFare,
                Status = PaymentStatus.Paid,
                PaidAt = DateTime.Now
            };

            rental.Status = RentalStatus.Completed;

            _db.Payments.Add(payment);

            _db.SaveChanges();

            return RedirectToAction(nameof(PaymentSuccess), new { id = payment.PaymentId });
        }

        public IActionResult PaymentSuccess(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Auth");

            var payment = _db.Payments
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.Vehicle)
                        .ThenInclude(v => v!.Category)
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.StartStation)
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.DestinationStation)
                .FirstOrDefault(p => p.PaymentId == id && p.UserId == CurrentUserId);

            if (payment == null) return NotFound();

            ViewBag.CurrentUser = GetCurrentUser();

            return View(payment);
        }

        private bool IsValidPaymentMethod(UserRole role, PaymentMethod method)
        {
            if (role == UserRole.LocalCommuter)
            {
                return method == PaymentMethod.Wallet
                    || method == PaymentMethod.MoMo
                    || method == PaymentMethod.VNPay
                    || method == PaymentMethod.Cash;
            }

            if (role == UserRole.ForeignTourist)
            {
                return method == PaymentMethod.ApplePay
                    || method == PaymentMethod.PayPal
                    || method == PaymentMethod.Cash;
            }

            return false;
        }
    }
}