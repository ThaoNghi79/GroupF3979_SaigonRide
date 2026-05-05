using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaigonRide.Data;
using SaigonRide.Models;

namespace SaigonRide.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int CurrentUserId = 2;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int rentalId, PaymentMethod method)
        {
            var rental = await _context.Rentals
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId && r.UserId == CurrentUserId);

            if (rental == null) return NotFound();

            if (rental.Payment == null)
            {
                _context.Payments.Add(new Payment
                {
                    RentalId = rental.RentalId,
                    UserId = rental.UserId,
                    Method = method,
                    Amount = rental.FinalFare,
                    Status = PaymentStatus.Paid,
                    PaidAt = DateTime.Now
                });
            }
            else
            {
                rental.Payment.Method = method;
                rental.Payment.Amount = rental.FinalFare;
                rental.Payment.Status = PaymentStatus.Paid;
                rental.Payment.PaidAt = DateTime.Now;
            }

            rental.Status = RentalStatus.Completed;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Success), new { id = rental.RentalId });
        }

        public async Task<IActionResult> Success(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Payment)
                .Include(r => r.Vehicle).ThenInclude(v => v!.Category)
                .Include(r => r.StartStation)
                .Include(r => r.DestinationStation)
                .FirstOrDefaultAsync(r => r.RentalId == id);

            if (rental == null) return NotFound();
            return View(rental);
        }
    }
}
