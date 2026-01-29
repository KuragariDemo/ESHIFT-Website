using System.ComponentModel.DataAnnotations;

namespace EShift.Models
{
    public class CustomerOrder
    {
        public CustomerOrder()
        {
            Status = "Pending"; // Set default status here
        }

        [Key]
        public int OrderId { get; set; }

        public string? UserId { get; set; }

        [Required]
        public string FromAddress { get; set; }

        [Required]
        public string ToAddress { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UnitType { get; set; }

        public List<AssignedJob> AssignedJobs { get; set; } = new();

        [Required]
        public string Status { get; set; } // No default here anymore

        public string? CarNumber { get; set; }
        public string? CarModel { get; set; }
        public string? DriverName { get; set; }
        public string? AssistantsName { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
    }

}
