using Microsoft.AspNetCore.Mvc;

namespace restaurant_pos_system.Controllers
{
    public class WaitronController : Controller
    {
        // GET: /Waitron/MyTables
        public IActionResult MyTables()
        {
            return View();
        }

        // GET: /Waitron/CreateOrder
        public IActionResult CreateOrder(int tableId)
        {
            ViewBag.TableId = tableId;
            return View();
        }
    }   
}