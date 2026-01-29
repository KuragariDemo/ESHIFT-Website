using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EShift.Models;
using EShift.Data;
using System.Threading.Tasks;

namespace EShift.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        // Inject UserManager and ApplicationDbContext
        public AdminDashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> AdminProfile()
        {
            // Get the current logged-in admin user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            // Fetch the admin data (you can customize this query based on your actual Admin table structure)
            var admin = await _context.Admins
                                      .FirstOrDefaultAsync(a => a.Email == currentUser.Email);

            if (admin == null)
            {
                return NotFound("Admin data not found");
            }

            // Create the view model and populate it with data
            var model = new AdminProfileViewModel
            {
                Id = admin.Id,
                Email = currentUser.Email,
                AdminLevel = admin.AdminLevel,
                NewPassword = string.Empty, // Placeholder for password field
                ConfirmNewPassword = string.Empty // Placeholder for password confirmation
            };

            // Return the view with the populated model
            return View(model);
        }
    }
}
