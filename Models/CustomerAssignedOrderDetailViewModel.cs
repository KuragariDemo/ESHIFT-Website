namespace EShift.Models
{
    public class CustomerAssignedOrderDetailViewModel
    {
        public int OrderId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string UserId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }

        public int CarId { get; set; }
        public string CarLicNo { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string DriverName { get; set; }
        public string AssistantName { get; set; }
        public int AssistantId { get; set; }
        public string Status { get; set; } 

    }


}
