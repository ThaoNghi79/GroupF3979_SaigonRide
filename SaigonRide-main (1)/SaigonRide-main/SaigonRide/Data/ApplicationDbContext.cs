using Microsoft.EntityFrameworkCore;
using SaigonRide.Models;
using System.Reflection.Emit;

namespace SaigonRide.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleCategory> VehicleCategories { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rental → StartStation (no cascade)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.StartStation)
                .WithMany()
                .HasForeignKey(r => r.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rental → DestinationStation (no cascade)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.DestinationStation)
                .WithMany()
                .HasForeignKey(r => r.DestinationStationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rental → Vehicle (no cascade)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Rentals)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment → Rental (one-to-one)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Rental)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── SEED DATA ───────────────────────────────────

            // Vehicle Categories
            modelBuilder.Entity<VehicleCategory>().HasData(
                new VehicleCategory { CategoryId = 1, CategoryName = "Standard Bike", PricePerMinute = 500 },
                new VehicleCategory { CategoryId = 2, CategoryName = "E-Scooter", PricePerMinute = 1500 }
            );

            // Stations
            modelBuilder.Entity<Station>().HasData(
                new Station { StationId = 1, StationName = "Ben Thanh Market", Location = "District 1", Capacity = 50, CurrentInventory = 25 },
                new Station { StationId = 2, StationName = "Nguyen Hue Walking St", Location = "District 1", Capacity = 45, CurrentInventory = 11 },
                new Station { StationId = 3, StationName = "Landmark 81", Location = "Binh Thanh District", Capacity = 25, CurrentInventory = 1 },
                new Station { StationId = 4, StationName = "Tan Hung", Location = "District 7", Capacity = 40, CurrentInventory = 7 },
                new Station { StationId = 5, StationName = "Phu Nhuan Station", Location = "Phu Nhuan District", Capacity = 30, CurrentInventory = 2 },
                new Station { StationId = 6, StationName = "Thu Duc Station", Location = "Thu Duc City", Capacity = 50, CurrentInventory = 48 },
                new Station { StationId = 7, StationName = "Bui Vien Street", Location = "District 1", Capacity = 20, CurrentInventory = 10 },
                new Station { StationId = 8, StationName = "Notre-Dame Cathedral", Location = "District 3", Capacity = 35, CurrentInventory = 20 }
            );

            // Vehicles
            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { VehicleId = 1, VehicleCode = "SB-V3-0017", VehicleName = "Bike VN-01", Status = VehicleStatus.Available, CategoryId = 1, StationId = 1 },
                new Vehicle { VehicleId = 2, VehicleCode = "ES-F4-0042", VehicleName = "E-Scooter CT-01", Status = VehicleStatus.InTransit, CategoryId = 2, StationId = 4 },
                new Vehicle { VehicleId = 3, VehicleCode = "ES-A1-0079", VehicleName = "Bike AG-01", Status = VehicleStatus.Maintenance, CategoryId = 1, StationId = 2 },
                new Vehicle { VehicleId = 4, VehicleCode = "SB-M1-001", VehicleName = "E-Scooter CM-01", Status = VehicleStatus.Maintenance, CategoryId = 2, StationId = 3 },
                new Vehicle { VehicleId = 5, VehicleCode = "SB-NS-0339", VehicleName = "Bike TN-01", Status = VehicleStatus.InTransit, CategoryId = 1, StationId = 4 },
                new Vehicle { VehicleId = 6, VehicleCode = "ES-E7-8386", VehicleName = "E-Scooter DB-01", Status = VehicleStatus.InTransit, CategoryId = 2, StationId = 5 },
                new Vehicle { VehicleId = 7, VehicleCode = "SB-T9-0099", VehicleName = "Bike TW-01", Status = VehicleStatus.Available, CategoryId = 1, StationId = 7 },
                new Vehicle { VehicleId = 8, VehicleCode = "ES-D2-0011", VehicleName = "E-Scooter VN-01", Status = VehicleStatus.Available, CategoryId = 2, StationId = 1 },
                new Vehicle { VehicleId = 9, VehicleCode = "SB-K1-0055", VehicleName = "Bike BV-01", Status = VehicleStatus.Available, CategoryId = 1, StationId = 7 },
                new Vehicle { VehicleId = 10, VehicleCode = "ES-L3-0088", VehicleName = "E-Scooter ND-01", Status = VehicleStatus.Available, CategoryId = 2, StationId = 8 }
            );

            // Admin user (password: Admin@123)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Thao Nghi Admin",
                    Email = "admin@saigonride.vn",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = UserRole.Admin,
                    WalletBalance = 0
                },
                new User
                {
                    UserId = 2,
                    FullName = "Nguyen Van A",
                    Email = "local@saigonride.vn",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    Role = UserRole.LocalCommuter,
                    WalletBalance = 500000
                },
                new User
                {
                    UserId = 3,
                    FullName = "Chris Evans",
                    Email = "tourist@saigonride.vn",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    Role = UserRole.ForeignTourist,
                    PassportNumber = "US123456",
                    IsPassportVerified = true,
                    WalletBalance = 1000000
                }
            );
        }
    }
}
