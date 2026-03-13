using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_pos_system.Models;
using System.Linq;
using System.Threading.Tasks;

namespace restaurant_pos_system.Controllers
{
    [Authorize(Roles = "Kitchen")]
    public class KitchenController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KitchenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Kitchen/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Get all orders that are not completed or paid
            var orders = await _context.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status != "Completed" && o.Status != "Paid")
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // GET: /Kitchen/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Waitron)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: /Kitchen/UpdateItemStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItemStatus(int orderItemId, string status)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderItemId);
            if (orderItem == null)
                return NotFound();

            orderItem.KitchenStatus = status;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = orderItem.OrderId });
        }
    }
}