using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_pos_system.Models;

namespace restaurant_pos_system.Controllers
{
    public class KitchenController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KitchenController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var orders = await _context.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status != "Completed" && o.Status != "Paid" && o.Status != "Cancelled")
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Waitron)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            ViewBag.ETA = order.Status == "Preparing" ? 15 : 20;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItemStatus(int orderItemId, string status)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderItemId);
            if (orderItem == null) return NotFound();

            var allowed = new[] { "Pending", "Cooking", "Ready" };
            if (!allowed.Contains(status)) return BadRequest();

            orderItem.KitchenStatus = status;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = orderItem.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            var allowed = new[] { "Pending", "Preparing", "Ready", "Completed", "Cancelled" };
            if (!allowed.Contains(status)) return BadRequest();

            order.Status = status;
            await _context.SaveChangesAsync();

            return status == "Cancelled"
                ? RedirectToAction("Dashboard")
                : RedirectToAction("Details", new { id = orderId });
        }
    }
}