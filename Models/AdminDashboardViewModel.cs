namespace EShift.Models
{
    public class AdminDashboardViewModel
    {
        public int PendingOrders { get; set; }
        public int AcceptedOrders { get; set; }
        public int FinishedOrders { get; set; }

        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int BusyCars { get; set; }
        public int UnderMaintenanceCars { get; set; }

        public int TotalAssistants { get; set; }
        public int AvailableAssistants { get; set; }
        public int BusyAssistants { get; set; }
        public int LeaveAssistants { get; set; }
    }
}
