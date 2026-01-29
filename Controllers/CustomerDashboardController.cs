using EShift.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Hosting; 

namespace EShift.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public CustomerDashboardController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        // GET: /CustomerDashboard/CusProfile
        [HttpGet]
        public async Task<IActionResult> CusProfile()
        {
            
            var user = await _userManager.Users
                               .Include(u => u.Customer) // Include the Customer navigation property
                               .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null || user.Customer == null) // Check if user or customer data is missing
            {
                
                return RedirectToAction("Login", "Account");
            }

            var model = new CustomerProfileViewModel
            {
                Name = user.Customer.Name, // Access Name from the Customer object
                ProfilePictureUrl = user.Customer.ProfilePicturePath // Access ProfilePicturePath from the Customer object
            };

            return View(model);
        }

        // POST: /CustomerDashboard/CusProfile
        [HttpPost]
        public async Task<IActionResult> CusProfile(CustomerProfileViewModel model)
        {
            // Eager load the Customer data
            var user = await _userManager.Users
                               .Include(u => u.Customer) // Include the Customer navigation property
                               .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null || user.Customer == null)
            {
                ModelState.AddModelError("", "User or customer profile not found.");
                return View(model);
            }

            // 1) Handle profile image upload
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.ProfileImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.Customer.ProfilePicturePath = "/uploads/" + fileName; // Update Customer's ProfilePicturePath
            }

            // 2) Update name
            user.Customer.Name = model.Name; // Update Customer's Name

            // 3) Update password if provided (this part still uses _userManager on ApplicationUser)
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

           
            var updateResult = await _userManager.UpdateAsync(user); // This saves changes to ApplicationUser and its tracked related entities
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("CusProfile");
        }
    }
}