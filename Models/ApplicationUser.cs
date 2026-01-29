using Microsoft.AspNetCore.Identity;
using EShift.Models;
namespace EShift.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Customer? Customer { get; set; }

    }
}
