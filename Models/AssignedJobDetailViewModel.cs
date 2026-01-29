namespace EShift.Models
{
    public class AssistantInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AssignedJobDetailViewModel
    {
        public int Id { get; set; }

        // Order Info
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string CustomerName { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Car Info
        public int CarId { get; set; }
        public string CarLicenseNo { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }

        // Driver Info
        public string DriverName { get; set; }

        // Assistant Info
        public List<AssistantInfo> Assistants { get; set; } = new();

        // Add the Status property
        public string Status { get; set; }
        public DateTime ScheduledDateTime { get; set; }
    }
}
