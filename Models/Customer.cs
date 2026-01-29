using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShift.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; } 

        // Foreign Key to ApplicationUser
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } 

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        public string? ProfilePicturePath { get; set; }

        // Add any other customer-specific properties here (e.g., Address, Phone Number)
    }
}