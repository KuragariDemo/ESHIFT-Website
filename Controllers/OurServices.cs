using Microsoft.AspNetCore.Mvc;

namespace EShift.Controllers
{
    public class OurServices : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
