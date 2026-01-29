namespace EShift.Models
{
    public class AssignedJobViewModel
    {
        public int Id { get; set; }            // AssignedJob.Id
        public int OrderId { get; set; }       // CustomerOrder.Id
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CarModel { get; set; }
        public string DriverName { get; set; }
        public List<string> AssistantNames { get; set; }
        public DateTime ScheduledDateTime { get; set; } // ✅ Required in DB
    }
}
