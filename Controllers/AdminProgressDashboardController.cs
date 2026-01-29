using EShift.Data; // Your DbContext namespace
using EShift.Models; // For your model classes
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EShift.Controllers
{
    public class AdminProgressDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminProgressDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminDashboard()
        {
            // ORDER COUNTS
            var pendingOrders = _context.CustomerOrders.Count(o => o.Status == "Pending");
            var acceptedOrders = _context.CustomerOrders.Count(o => o.Status == "Accepted");
            var finishedOrders = _context.CustomerOrders.Count(o => o.Status == "Finished");

            // CAR COUNTS
            var totalCars = _context.Cars.Count();
            var availableCars = _context.Cars.Count(c => c.Status == "Available");
            var busyCars = _context.Cars.Count(c => c.Status == "Busy");
            var underMaintenanceCars = _context.Cars.Count(c => c.Status == "Under Maintenance");

            // ASSISTANT COUNTS
            var totalAssistants = _context.Assistants.Count();
            var availableAssistants = _context.Assistants.Count(a => a.Status == "Available");
            var busyAssistants = _context.Assistants.Count(a => a.Status == "Busy");
            var leaveAssistants = _context.Assistants.Count(a => a.Status == "Leave");

            var viewModel = new AdminDashboardViewModel
            {
                PendingOrders = pendingOrders,
                AcceptedOrders = acceptedOrders,
                FinishedOrders = finishedOrders,

                TotalCars = totalCars,
                AvailableCars = availableCars,
                BusyCars = busyCars,
                UnderMaintenanceCars = underMaintenanceCars,

                TotalAssistants = totalAssistants,
                AvailableAssistants = availableAssistants,
                BusyAssistants = busyAssistants,
                LeaveAssistants = leaveAssistants
            };

            return View("~/Views/AdminDashboard/AdminDashboard.cshtml", viewModel);
        }

    }
}
