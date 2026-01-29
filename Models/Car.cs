namespace EShift.Models
{
    public class Car
    {
        public int CarID { get; set; }

        public string CarType { get; set; } // "Small", "Medium", "Large"
        public string CarLicenseNo { get; set; } // License Number
        public string CarModel { get; set; } // e.g. "Toyota Hilux"

        public string Status { get; set; } // "Available", "Busy", "Under Maintenance"
        public string? AssignedJobId { get; set; } // Show Job ID or "NON"
    }
}
