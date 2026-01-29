// Controllers/CustomerOrderController.cs
using Microsoft.AspNetCore.Mvc;
using EShift.Data;
using EShift.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EShift.Controllers
{
    public class CustomerOrderController : Controller
    {

        private readonly ApplicationDbContext _context; 
        public CustomerOrderController(ApplicationDbContext context) 
        
        {
            _context = context;
        }
        public IActionResult CusOrder()
        {
            return View("~/Views/CustomerDashboard/CusOrder.cshtml");
        }

        // Show all orders
        public IActionResult CusOrderList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = _context.CustomerOrders
                         .Where(o => o.UserId == userId)
                         .AsNoTracking()
                         .ToList();

            return View("~/Views/CustomerDashboard/CusOrderList.cshtml", orders);
        }


        // GET: Edit Order
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.CustomerOrders.FindAsync(id);
            if (order == null) return NotFound();
            return View("~/Views/CustomerDashboard/CusOrderEdit.cshtml", order);
        }

        // GET: Detail Order
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _context.CustomerOrders
                .Include(o => o.AssignedJobs) // Include the related AssignedJobs
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            // Now, you can access order.AssignedJobs
            var cars = _context.Cars.ToList();
            var assistants = _context.Assistants.ToList();

            var assignedJobDetails = order.AssignedJobs.Select(job => new CustomerAssignedOrderDetailViewModel
            {
                OrderId = order.OrderId,
                FromAddress = order.FromAddress,
                ToAddress = order.ToAddress,
                UserId = order.UserId,
                CustomerName = order.Name,
                Phone = order.PhoneNumber,
                CarId = cars.FirstOrDefault(c => c.CarID == job.CarId)?.CarID ?? 0,
                CarLicNo = cars.FirstOrDefault(c => c.CarID == job.CarId)?.CarLicenseNo ?? "N/A",
                CarModel = cars.FirstOrDefault(c => c.CarID == job.CarId)?.CarModel ?? "N/A",
                CarType = cars.FirstOrDefault(c => c.CarID == job.CarId)?.CarType ?? "N/A",
                DriverName = assistants.FirstOrDefault(a => a.AssistantID == job.DriverId)?.AssistantName ?? "N/A",
                AssistantId = int.TryParse(job.LoaderIds?.Split(',').FirstOrDefault(), out var id) ? id : 0,
                AssistantName = assistants.FirstOrDefault(a => a.AssistantID.ToString() == job.LoaderIds)?.AssistantName ?? "N/A"
            }).ToList();

            return View("~/Views/CustomerDashboard/CustomerAssignedOrderDetail.cshtml", assignedJobDetails);
        }




        // POST: Save form data
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("FromAddress,ToAddress,Email,PhoneNumber,Name,UnitType,Status")] CustomerOrder order)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br/>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Content("Validation Failed:<br/>" + errors);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                order.UserId = userId;

                if (string.IsNullOrEmpty(order.Status))
                {
                    order.Status = "Pending";
                }

                _context.CustomerOrders.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("CusOrderList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("CusOrderList");
            }
        }

        // POST: Edit Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerOrder order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOrder = await _context.CustomerOrders.FindAsync(id);
                    if (existingOrder == null)
                    {
                        return NotFound();
                    }

                    // Update only editable fields
                    existingOrder.FromAddress = order.FromAddress;
                    existingOrder.ToAddress = order.ToAddress;
                    existingOrder.Email = order.Email;
                    existingOrder.PhoneNumber = order.PhoneNumber;
                    existingOrder.Name = order.Name;
                    existingOrder.UnitType = order.UnitType;

                    await _context.SaveChangesAsync();

                    return RedirectToAction("CusOrderList");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CustomerOrders.Any(e => e.OrderId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View("CusOrderEdit", order);
        }



    }

}
