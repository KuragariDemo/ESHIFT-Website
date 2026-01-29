// EShift.Models.AdminProfileViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace EShift.Models
{
    public class AdminProfileViewModel
    {
        public int Id { get; set; } // Assuming an Id is needed for updates

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Admin Level")]
        public int AdminLevel { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        // Make it nullable if leaving blank means no change
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}