using Microsoft.AspNetCore.Mvc;

namespace Consume.Controllers
{
    public class PanelController : Controller
    {
        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();
        }
    }
}
