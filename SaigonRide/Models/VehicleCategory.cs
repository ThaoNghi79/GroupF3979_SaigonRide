using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaigonRide.Models
{

    public enum VehicleStatus { Available, InTransit, Maintenance }
    public enum UserRole { LocalCommuter, ForeignTourist, Admin }
    public enum RentalStatus { Active, PendingPayment, Completed, Cancelled }
    public enum PaymentStatus { Pending, Paid, Failed, Cancelled }
    public enum PaymentMethod { MoMo, VNPay, Cash, ApplePay, PayPal }


    public class VehicleCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, MaxLength(50)]
        public string CategoryName { get; set; } = "";   

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerMinute { get; set; }      

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

    public class Station
    {
        public int StationId { get; set; }

        [Required, MaxLength(100)]
        public string StationName { get; set; } = "";

        [Required, MaxLength(200)]
        public string Location { get; set; } = "";

        [Range(1, 500)]
        public int Capacity { get; set; }

        public int CurrentInventory { get; set; } = 0;

        public string? Status { get; set; } = "Active";

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public bool IsLowInventory()
        {
            if (Capacity == 0) return false;
            return (double)CurrentInventory / Capacity < 0.20;
        }

        [NotMapped]
        public double UtilizationPercent =>
            Capacity > 0 ? Math.Round((double)CurrentInventory / Capacity * 100, 1) : 0;
    }


    public class Vehicle
    {
        public int VehicleId { get; set; }

        [Required, MaxLength(20)]
        public string VehicleCode { get; set; } = "";    

        [Required, MaxLength(100)]
        public string VehicleName { get; set; } = "";   

        public VehicleStatus Status { get; set; } = VehicleStatus.Available;


        public int CategoryId { get; set; }
        public VehicleCategory? Category { get; set; }

        public int StationId { get; set; }
        public Station? Station { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        public string? ImageUrl { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = "";

        [Required, MaxLength(150)]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        public UserRole Role { get; set; } = UserRole.LocalCommuter;


        [MaxLength(20)]
        public string? PassportNumber { get; set; }
        public bool IsPassportVerified { get; set; } = false;

        [Column(TypeName = "decimal(15,2)")]
        public decimal WalletBalance { get; set; } = 0;

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        public string? AvatarUrl { get; set; }
        public bool AlertStationOverload { get; set; } = false;
        public bool AlertVehicleMaintenance { get; set; } = false;
        public bool AlertStationInventory { get; set; } = false;
        public bool IsLocked { get; set; } = false;
    }

    public class Rental
    {
        public int RentalId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int StartStationId { get; set; }
        public Station? StartStation { get; set; }

        public int? DestinationStationId { get; set; }
        public Station? DestinationStation { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationMinutes { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal BaseFare { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal FinalFare { get; set; }

        public RentalStatus Status { get; set; } = RentalStatus.Active;

        public Payment? Payment { get; set; }
    }


    public class Payment
    {
        public int PaymentId { get; set; }

        public int RentalId { get; set; }
        public Rental? Rental { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public PaymentMethod Method { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime? PaidAt { get; set; }
    }
}