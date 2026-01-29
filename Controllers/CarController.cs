using EShift.Data;
using EShift.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync()
public class CarController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        var cars = from c in _context.Cars
                   select c;

        if (!string.IsNullOrEmpty(searchString))
        {
            cars = cars.Where(c =>
                c.CarType.Contains(searchString) ||
                c.CarLicenseNo.Contains(searchString) ||
                c.CarModel.Contains(searchString)
            );
        }

        ViewData["CurrentFilter"] = searchString;

        return View("~/Views/AdminDashboard/Car.cshtml", await cars.ToListAsync());
    }


    public IActionResult Create()
    {
        return View("~/Views/AdminDashboard/CarCreate.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Car car)
    {
        if (ModelState.IsValid)
        {
            car.AssignedJobId = "NON";
            _context.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View("~/Views/AdminDashboard/CarCreate.cshtml", car);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        return View("~/Views/AdminDashboard/CarEdit.cshtml", car);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Car car)
    {
        if (id != car.CarID)
            return NotFound();

        var existing = await _context.Cars.FindAsync(id);
        if (existing == null)
            return NotFound();

        // Block manual setting to Busy
        if (car.Status == "Busy" && existing.Status != "Busy")
        {
            ModelState.AddModelError("Status", "You cannot manually set the status to 'Busy'.");
        }

        // Block all status changes if car is currently busy or assigned
        bool isBusyOrAssigned = existing.Status == "Busy" || (!string.IsNullOrEmpty(existing.AssignedJobId) && existing.AssignedJobId != "NON");
        if (isBusyOrAssigned && car.Status != existing.Status)
        {
            ModelState.AddModelError("Status", "Cannot change status while car is busy or assigned.");
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/AdminDashboard/CarEdit.cshtml", car);
        }

        // Safe updates
        existing.CarType = car.CarType;
        existing.CarLicenseNo = car.CarLicenseNo;
        existing.CarModel = car.CarModel;

        // Only allow Available <-> Under Maintenance when not busy/assigned
        if (!isBusyOrAssigned && (car.Status == "Available" || car.Status == "Under Maintenance"))
        {
            existing.Status = car.Status;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        return View("~/Views/AdminDashboard/CarDelete.cshtml", car);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car != null)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

}

