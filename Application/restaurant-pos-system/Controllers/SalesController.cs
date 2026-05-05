using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_pos_system.Models;

namespace restaurant_pos_system.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SalesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == "Paid")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Receipt(int id)
        {
            var order = await _db.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}