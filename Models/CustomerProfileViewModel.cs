using Microsoft.AspNetCore.Http;

namespace EShift.Models
{
    public class CustomerProfileViewModel
    {
        public string Name { get; set; }
        public string? Password { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
