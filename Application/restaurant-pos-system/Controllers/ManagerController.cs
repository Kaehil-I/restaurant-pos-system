using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_pos_system.Models;

namespace restaurant_pos_system.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ManagerController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            ViewBag.TotalSalesToday = await _db.Payments
                .Where(p => p.PaidAt.Date == today)
                .SumAsync(p => p.TotalAmount);
            ViewBag.OrdersToday = await _db.Orders
                .Where(o => o.CreatedAt.Date == today)
                .CountAsync();
            ViewBag.ActiveOrders = await _db.Orders
                .Where(o => o.Status != "Paid" && o.Status != "Cancelled")
                .CountAsync();
            ViewBag.LowStock = await _db.InventoryItems
                .Where(i => i.Quantity <= i.ReorderLevel)
                .CountAsync();
            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _db.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Stock()
        {
            var items = await _db.InventoryItems.ToListAsync();
            if (!items.Any())
            {
                _db.InventoryItems.AddRange(
                    new InventoryItem { Name = "Beef Patties", Quantity = 50, Unit = "pcs", ReorderLevel = 10 },
                    new InventoryItem { Name = "Burger Buns", Quantity = 50, Unit = "pcs", ReorderLevel = 10 },
                    new InventoryItem { Name = "Chicken Breast", Quantity = 20, Unit = "kg", ReorderLevel = 5 },
                    new InventoryItem { Name = "Pizza Dough", Quantity = 15, Unit = "kg", ReorderLevel = 5 },
                    new InventoryItem { Name = "Lettuce", Quantity = 8, Unit = "kg", ReorderLevel = 3 },
                    new InventoryItem { Name = "Coke Cans", Quantity = 100, Unit = "pcs", ReorderLevel = 20 },
                    new InventoryItem { Name = "Water Bottles", Quantity = 5, Unit = "pcs", ReorderLevel = 20 }
                );
                await _db.SaveChangesAsync();
                items = await _db.InventoryItems.ToListAsync();
            }
            return View(items);
        }

        public async Task<IActionResult> Staff()
        {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Sales()
        {
            var payments = await _db.Payments
                .Include(p => p.Order).ThenInclude(o => o.OrderItems)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();

            var today = DateTime.Today;
            ViewBag.TodayTotal = payments.Where(p => p.PaidAt.Date == today).Sum(p => p.TotalAmount);
            ViewBag.AllTimeTotal = payments.Sum(p => p.TotalAmount);
            return View(payments);
        }

        // POST: Add stock item
        [HttpPost]
        public async Task<IActionResult> AddStock(string name, decimal quantity, string unit, decimal reorderLevel)
        {
            _db.InventoryItems.Add(new InventoryItem
            {
                Name = name,
                Quantity = quantity,
                Unit = unit,
                ReorderLevel = reorderLevel
            });
            await _db.SaveChangesAsync();
            return RedirectToAction("Stock");
        }

        // POST: Add reservation
        [HttpPost]
        public async Task<IActionResult> AddReservation(string customerName, string phoneNumber, DateTime reservationTime, int numberOfGuests, int tableId)
        {
            _db.Reservations.Add(new Reservation
            {
                CustomerName = customerName,
                PhoneNumber = phoneNumber,
                ReservationTime = reservationTime,
                NumberOfGuests = numberOfGuests,
                RestaurantTableId = tableId
            });
            var table = await _db.RestaurantTables.FindAsync(tableId);
            if (table != null) table.Status = "Reserved";
            await _db.SaveChangesAsync();
            return RedirectToAction("Reservations");
        }

        public async Task<IActionResult> Reservations()
        {
            var reservations = await _db.Reservations
                .Include(r => r.RestaurantTable)
                .OrderBy(r => r.ReservationTime)
                .ToListAsync();
            var tables = await _db.RestaurantTables.ToListAsync();
            ViewBag.Tables = tables;
            return View(reservations);
        }
    }
}