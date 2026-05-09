using System.ComponentModel.DataAnnotations;

namespace SaigonRide.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public SaigonRide.Models.Enums.UserRole Role { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }

        public bool IsPassVerified { get; set; }
        public string? PassportNumber { get; set; }
        public string? NationalId { get; set; }
        public decimal WalletBalance { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}

namespace SaigonRide.Models.Enums
{
    public enum UserRole
    {
        ForeignTourist,
        LocalCommuter,
        Admin
    }
}
