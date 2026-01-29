using System.ComponentModel.DataAnnotations.Schema;

namespace EShift.Models
{
    public class AssignedJob
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public CustomerOrder Order { get; set; }
        public string? UserId { get; set; }
        public int CarId { get; set; }
        public int DriverId { get; set; }
        public string LoaderIds { get; set; } // comma-separated string
        public DateTime ScheduledDateTime { get; set; }
        public string Status { get; set; }

    }

}
