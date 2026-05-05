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

        // GET: /Sales — lists all paid orders
        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == "Paid")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // GET: /Sales/Receipt/5
        public async Task<IActionResult> Receipt(int id)
        {
            var order = await _db.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // GET: /Sales/Summary — daily totals
        public async Task<IActionResult> Summary()
        {
            var today = DateTime.Today;

            var payments = await _db.Payments
                .Include(p => p.Order)
                .Where(p => p.PaidAt.Date == today)
                .ToListAsync();

            ViewBag.TotalSales = payments.Sum(p => p.TotalAmount);
            ViewBag.TotalOrders = payments.Count;
            ViewBag.CashTotal = payments.Where(p => p.PaymentMethod == "Cash").Sum(p => p.TotalAmount);
            ViewBag.CardTotal = payments.Where(p => p.PaymentMethod == "Card").Sum(p => p.TotalAmount);

            return View(payments);
        }
    }
}