// Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EShift.Data;
using EShift.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EShift.Controllers
{
    // Ensure only authenticated users with the "Admin" role can access this controller
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context,
                               UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin
        // Displays a list of all admin accounts
        public async Task<IActionResult> Index()
        {
            // Get the current logged-in admin's email and level
            var currentAdminEmail = User.Identity.Name;
            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == currentAdminEmail);

            // Pass the current admin's level to the view for conditional rendering
            ViewBag.CurrentAdminLevel = currentAdmin?.AdminLevel ?? 0;

            var admins = await _context.Admins.ToListAsync();
            // Explicitly return the view from Views/AdminDashboard/AdminControl.cshtml
            return View("~/Views/AdminDashboard/AdminControl.cshtml", admins);
        }

        // GET: Admin/Register
        // Displays the form to register a new admin account
        public IActionResult Register()
        {
            // Check if the current admin has AdminLevel 1
            var currentAdminEmail = User.Identity.Name;
            var currentAdmin = _context.Admins.FirstOrDefault(a => a.Email == currentAdminEmail);

            if (currentAdmin == null || currentAdmin.AdminLevel != 1)
            {
                return Forbid();
            }

            // Explicitly return the view from Views/AdminDashboard/AdminRegister.cshtml
            return View("~/Views/AdminDashboard/AdminRegister.cshtml");
        }

        // POST: Admin/Register
        // Handles the submission of the new admin registration form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterAdminViewModel model)
        {
            // Check if the current admin has AdminLevel 1 again for security
            var currentAdminEmail = User.Identity.Name;
            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == currentAdminEmail);

            if (currentAdmin == null || currentAdmin.AdminLevel != 1)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign the "Admin" role to the new user
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    await _userManager.AddToRoleAsync(user, "Admin");

                    // Add the admin details to your custom Admin table
                    var admin = new Admin
                    {
                        Email = model.Email,
                        AdminLevel = model.AdminLevel
                    };
                    _context.Admins.Add(admin);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Admin account registered successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If model state is invalid, return the view with errors
            return View("~/Views/AdminDashboard/AdminRegister.cshtml", model);
        }

        // GET: Admin/Edit/5
        // Displays the form to edit an existing admin account
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if the current admin has AdminLevel 1
            var currentAdminEmail = User.Identity.Name;
            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == currentAdminEmail);

            if (currentAdmin == null || currentAdmin.AdminLevel != 1)
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            var model = new EditAdminViewModel
            {
                Id = admin.Id,
                Email = admin.Email,
                AdminLevel = admin.AdminLevel
            };
            // Explicitly return the view from Views/AdminDashboard/AdminEdit.cshtml
            return View("~/Views/AdminDashboard/AdminEdit.cshtml", model);
        }

        // POST: Admin/Edit/5
        // Handles the submission of the edit admin form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAdminViewModel model)
        {
            // Check if the current admin has AdminLevel 1
            var currentAdminEmail = User.Identity.Name;
            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == currentAdminEmail);

            if (currentAdmin == null || currentAdmin.AdminLevel != 1)
            {
                return Forbid();
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var adminToUpdate = await _context.Admins.FindAsync(id);
                    if (adminToUpdate == null)
                    {
                        return NotFound();
                    }

                    adminToUpdate.AdminLevel = model.AdminLevel;

                    _context.Update(adminToUpdate);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Admin account updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Admins.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            // If model state is invalid, return the view with errors
            return View("~/Views/AdminDashboard/AdminEdit.cshtml", model);
        }

        // GET: Admin/Delete/5
        // Displays the confirmation page for deleting an admin account
        public async Task<IActionResult> Delete(int? id)
        {
            // Check if the current admin has AdminLevel 1
            var currentAdminEmail = User.Identity.Name;
            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == currentAdminEmail);

            if (currentAdmin == null || currentAdmin.AdminLevel != 1)
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            // Explicitly return the view from Views/AdminDashboard/AdminDelete.cshtml
            return View("~/Views/AdminDashboard/AdminDelete.cshtml", admin);
        }

        // POST: Admin/Delete/5
        // Handles the actual deletion of an admin account after confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if the current admin has AdminLevel 1
            var currentAdminEmail = User.Identity.Name;
            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == currentAdminEmail);

            if (currentAdmin == null || currentAdmin.AdminLevel != 1)
            {
                return Forbid();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            // Also delete the user from ASP.NET Core Identity
            var identityUser = await _userManager.FindByEmailAsync(admin.Email);
            if (identityUser != null)
            {
                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "Error deleting Identity user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Index));
                }
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin account deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Helper method to get the current admin's level
        private async Task<int> GetCurrentAdminLevel()
        {
            var currentAdminEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(currentAdminEmail)) return 0;

            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == currentAdminEmail);
            return currentAdmin?.AdminLevel ?? 0;
        }
    }
}