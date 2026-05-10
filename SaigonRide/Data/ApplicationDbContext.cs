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


            modelBuilder.Entity<Rental>()
                .HasOne(r => r.StartStation)
                .WithMany()
                .HasForeignKey(r => r.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.DestinationStation)
                .WithMany()
                .HasForeignKey(r => r.DestinationStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Rentals)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Rental)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RentalId)
                .OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<VehicleCategory>().HasData(
                new VehicleCategory { CategoryId = 1, CategoryName = "Standard Bike", PricePerMinute = 500 },
                new VehicleCategory { CategoryId = 2, CategoryName = "E-Scooter", PricePerMinute = 1500 }
            );

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
