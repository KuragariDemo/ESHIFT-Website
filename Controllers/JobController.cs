using EShift.Data;
using EShift.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShift.Controllers
{
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Job/Assign?orderId=123
        public async Task<IActionResult> Assign(int orderId)
        {
            var order = await _context.CustomerOrders.FindAsync(orderId);
            if (order == null) return NotFound();

            var vm = new AssignJobViewModel
            {
                Order = order,
                AvailableCars = await _context.Cars
                    .Where(c => c.Status == "Available")
                    .ToListAsync(),
                AvailableDrivers = await _context.Assistants
                    .Where(a => a.Status == "Available" && a.Role == "Driver")
                    .ToListAsync(),
                AvailableLoaders = await _context.Assistants
                    .Where(a => a.Status == "Available" && a.Role == "Loader")
                    .ToListAsync(),
                ScheduledDate = DateTime.Now.Date,
                ScheduledTime = TimeSpan.Zero
            };

            return View("~/Views/AdminDashboard/JobAssign.cshtml", vm);
        }

        // POST: /Job/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignJobViewModel vm)
        {

            Console.WriteLine("POST Assign Triggered");
            Console.WriteLine($"Car: {vm.SelectedCarId}, Driver: {vm.SelectedDriverId}, Loaders: {vm.SelectedLoaderIds}");
            // ✅ Remove non-input properties from validation
            ModelState.Remove(nameof(vm.Order));
            ModelState.Remove(nameof(vm.AvailableCars));
            ModelState.Remove(nameof(vm.AvailableDrivers));
            ModelState.Remove(nameof(vm.AvailableLoaders));

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(",", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                // 🧠 Refill all needed data so the view won't throw null exception
                vm.Order = await _context.CustomerOrders.FindAsync(vm.OrderId);

                vm.AvailableCars = await _context.Cars
                    .Where(c => c.Status == "Available").ToListAsync();
                vm.AvailableDrivers = await _context.Assistants
                    .Where(a => a.Status == "Available" && a.Role == "Driver").ToListAsync();
                vm.AvailableLoaders = await _context.Assistants
                    .Where(a => a.Status == "Available" && a.Role == "Loader").ToListAsync();

                return View("~/Views/AdminDashboard/JobAssign.cshtml", vm);
            }

            var order = await _context.CustomerOrders.FindAsync(vm.OrderId);

            if (order == null) return NotFound();

            // ✅ Update order status to Accepted (not Pending)
            order.Status = "Accepted";
            _context.CustomerOrders.Update(order);

            // ✅ Mark selected car as busy
            var car = await _context.Cars.FindAsync(vm.SelectedCarId);
            if (car != null)
            {
                car.Status = "Busy";
                car.AssignedJobId = vm.OrderId.ToString();
                _context.Cars.Update(car);
            }

            // ✅ Mark driver as busy
            var driver = await _context.Assistants.FindAsync(vm.SelectedDriverId);
            if (driver != null)
            {
                driver.Status = "Busy";
                driver.AssignedJobId = vm.OrderId.ToString();
                _context.Assistants.Update(driver);
            }

            // ✅ Parse the comma-separated string into List<int>
            var loaderIdList = vm.SelectedLoaderIds?
                .Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(int.Parse)
                .ToList() ?? new List<int>();

            // ✅ Use the parsed list
            if (loaderIdList.Any())
            {
                foreach (var lid in loaderIdList)
                {
                    var loader = await _context.Assistants.FindAsync(lid);
                    if (loader != null)
                    {
                        loader.Status = "Busy";
                        loader.AssignedJobId = vm.OrderId.ToString();
                        _context.Assistants.Update(loader);
                    }
                }
            }

            // ✅ Save assigned job
            var assignedJob = new AssignedJob
            {
                OrderId = vm.OrderId,
                UserId = order.UserId,
                CarId = vm.SelectedCarId,
                DriverId = vm.SelectedDriverId,
                LoaderIds = string.Join(",", loaderIdList), // Store loader IDs as comma-separated string
                ScheduledDateTime = vm.ScheduledDate + vm.ScheduledTime,
                Status = "Pending" // Add this line to set the status
            };


            _context.AssignedJobs.Add(assignedJob);

            // ✅ Commit all changes
            await _context.SaveChangesAsync();

            // ✅ Redirect to assigned job list or job summary
            return RedirectToAction("AssignedJobs", "Job");
        }

        public async Task<IActionResult> AssignedJobs()
        {
            var jobs = await _context.AssignedJobs.ToListAsync();
            var result = new List<AssignedJobViewModel>();

            foreach (var job in jobs)
            {
                var order = await _context.CustomerOrders.FindAsync(job.OrderId);
                var car = await _context.Cars.FindAsync(job.CarId);

                // ✅ Get driver by ID and Role
                var driver = await _context.Assistants
                    .Where(a => a.AssistantID == job.DriverId && a.Role == "Driver")
                    .FirstOrDefaultAsync();

                // ✅ Process loader names
                var loaderIds = job.LoaderIds?.Split(',') ?? Array.Empty<string>();
                var loaders = new List<string>();
                foreach (var lid in loaderIds)
                {
                    if (int.TryParse(lid, out var lidInt))
                    {
                        var loader = await _context.Assistants
                            .Where(a => a.AssistantID == lidInt && a.Role == "Loader")
                            .FirstOrDefaultAsync();

                        if (loader != null) loaders.Add(loader.AssistantName);
                    }
                }

                result.Add(new AssignedJobViewModel
                {
                    Id = job.Id,
                    OrderId = job.OrderId,
                    CustomerName = order?.Name ?? "N/A",
                    Email = order?.Email ?? "N/A",
                    Phone = order?.PhoneNumber ?? "N/A",
                    CarModel = car?.CarModel ?? "N/A",
                    DriverName = driver?.AssistantName ?? "N/A",
                    AssistantNames = loaders
                });
            }

            return View("~/Views/AdminDashboard/AssignedJobs.cshtml", result);
        }

        public async Task<IActionResult> AssignedJobDetail(int id)
        {
            var job = await _context.AssignedJobs.FindAsync(id);
            if (job == null) return NotFound();

            var order = await _context.CustomerOrders.FindAsync(job.OrderId);
            var car = await _context.Cars.FindAsync(job.CarId);
            var driver = await _context.Assistants.FindAsync(job.DriverId);

            var assistantList = new List<AssistantInfo>();
            var loaderIds = job.LoaderIds?.Split(',') ?? Array.Empty<string>();
            foreach (var lid in loaderIds)
            {
                if (int.TryParse(lid, out var lidInt))
                {
                    var loader = await _context.Assistants.FindAsync(lidInt);
                    if (loader != null)
                    {
                        assistantList.Add(new AssistantInfo
                        {
                            Id = loader.AssistantID,
                            Name = loader.AssistantName
                        });
                    }
                }
            }

            var vm = new AssignedJobDetailViewModel
            {
                Id = job.Id,
                OrderId = order.OrderId,
                UserId = order.UserId,
                CustomerName = order.Name,
                FromAddress = order.FromAddress,
                ToAddress = order.ToAddress,
                Email = order.Email,
                Phone = order.PhoneNumber,
                CarId = car.CarID,
                CarLicenseNo = car.CarLicenseNo,
                CarModel = car.CarModel,
                CarType = car.CarType,
                DriverName = driver?.AssistantName ?? "N/A",
                Assistants = assistantList,
                Status = job.Status,
                ScheduledDateTime = job.ScheduledDateTime
            };

            return View("~/Views/AdminDashboard/AssignedJobDetail.cshtml", vm);
        }
    }
}
