namespace EShift.Models
{
    public class Assistant
    {
        public int AssistantID { get; set; }
        public string AssistantName { get; set; }
        public string Status { get; set; } // "Available", "Busy", "Leave"
        public string Role { get; set; }   // "Driver" or "Loader"
        public string? AssignedJobId { get; set; } // Optional
    }

}
