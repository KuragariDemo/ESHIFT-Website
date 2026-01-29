using EShift.Data;
using EShift.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    

namespace EShift.Controllers
{
    public class AdminCustomerOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCustomerOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin View - List All Orders
        public async Task<IActionResult> Index(string searchString)
        {
            var orders = from o in _context.CustomerOrders
                         select o;

            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.Name.Contains(searchString) ||
                    o.PhoneNumber.Contains(searchString) ||
                    o.Email.Contains(searchString) ||
                    o.FromAddress.Contains(searchString) ||
                    o.ToAddress.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            return View("~/Views/AdminDashboard/CustomerOrders.cshtml", await orders.ToListAsync());
        }


        // Redirect to Assign Job
        public async Task<IActionResult> AssignJob(int id)
        {
            var order = await _context.CustomerOrders.FindAsync(id);
            if (order == null) return NotFound();

            return RedirectToAction("Assign", "Job", new { orderId = id });
        }



    }

}
