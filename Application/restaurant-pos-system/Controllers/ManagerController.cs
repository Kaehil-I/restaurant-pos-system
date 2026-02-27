using Microsoft.AspNetCore.Mvc;

namespace restaurant_pos_system.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult CreatedOrders()
        {
            return View();
        }

        public IActionResult Stock()
        {
            return View();
        }

        public IActionResult Staff()
        {
            return View();
        }
    }
}
