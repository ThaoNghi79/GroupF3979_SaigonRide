using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaigonRide.Models.ViewModels
{
    public class AdminProfileViewModel
    {

        public int UserId { get; set; }

        [Required(ErrorMessage = "Display name is required.")]
        [StringLength(100)]
        [Display(Name = "Display Name")]
        public string FullName { get; set; } = string.Empty;


        public string? Email { get; set; }
        public string? EmployeeId { get; set; }
        public string? AvatarUrl { get; set; }


        [Display(Name = "Station Overload / Empty Station")]
        public bool AlertStationOverload { get; set; }

        [Display(Name = "Vehicle Maintenance Alert")]
        public bool AlertVehicleMaintenance { get; set; }

        [Display(Name = "Station Low Inventory")]
        public bool AlertStationInventory { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Admin passwords must be at least 8 characters.")]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }


        public List<AdminActivityLogItem> ActivityLog { get; set; } = new();
    }

    public class AdminActivityLogItem
    {
        public int LogId { get; set; }
        public string? ActionType { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
    }
}
