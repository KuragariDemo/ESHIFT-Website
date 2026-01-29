namespace EShift.Models
{
    public class AdminCustomerOrders
    {
        public int Id { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string UnitType { get; set; } // Small, Medium, Large
        public string Status { get; set; } = "Pending"; // Pending / Accepted
    }
}
