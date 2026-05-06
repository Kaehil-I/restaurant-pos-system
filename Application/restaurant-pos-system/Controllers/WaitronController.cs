using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using restaurant_pos_system.Models;

namespace restaurant_pos_system.Controllers
{
    public class WaitronController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WaitronController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Waitron/MyTables
        public async Task<IActionResult> MyTables()
        {
            var tables = await _context.RestaurantTables.ToListAsync();
            return View(tables);
        }

        // GET: /Waitron/CreateOrder
        public async Task<IActionResult> CreateOrder(int tableId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o =>
                    o.RestaurantTableId == tableId &&
                    o.Status == "Open");

            if (order == null)
            {
                order = new Order
                {
                    RestaurantTableId = tableId,
                    WaitronId = userId,
                    CreatedAt = DateTime.Now,
                    Status = "Open",
                    OrderItems = new List<OrderItem>()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }

            ViewBag.MenuCategories = await _context.MenuCategories
                .Include(c => c.MenuItems)
                .ToListAsync();

            return View(order);
        }

        // POST: Add Item
        [HttpPost]
        public async Task<IActionResult> AddItem(int orderId, int menuItemId)
        {
            var menuItem = await _context.MenuItems.FindAsync(menuItemId);

            var existing = await _context.OrderItems
                .FirstOrDefaultAsync(o =>
                    o.OrderId == orderId &&
                    o.MenuItemId == menuItemId);

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = orderId,
                    MenuItemId = menuItemId,
                    Quantity = 1,
                    Price = menuItem.Price,
                    KitchenStatus = "Pending"
                });
            }

            await _context.SaveChangesAsync();

            // redirect back properly
            var order = await _context.Orders.FindAsync(orderId);

            return RedirectToAction("CreateOrder", new { tableId = order.RestaurantTableId });
        }

        // POST: Send to Kitchen
        [HttpPost]
        public async Task<IActionResult> SendToKitchen(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            foreach (var item in order.OrderItems)
            {
                item.KitchenStatus = "Pending";
            }

            order.Status = "InProgress";

            await _context.SaveChangesAsync();

            return RedirectToAction("MyTables");
        }
    }
}