using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_pos_system.Models;

namespace restaurant_pos_system.Controllers
{
    public class WaitronController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WaitronController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Waitron/MyTables
        public async Task<IActionResult> MyTables()
        {
            var tables = await _db.RestaurantTables
                .Include(t => t.Orders)
                .ToListAsync();

            if (!tables.Any())
            {
                // Seed tables if none exist
                for (int i = 1; i <= 8; i++)
                {
                    _db.RestaurantTables.Add(new RestaurantTable
                    {
                        TableNumber = i,
                        Capacity = 4,
                        Status = "Available"
                    });
                }
                await _db.SaveChangesAsync();
                tables = await _db.RestaurantTables.Include(t => t.Orders).ToListAsync();
            }

            return View(tables);
        }

        // GET: /Waitron/CreateOrder?tableId=1
        public async Task<IActionResult> CreateOrder(int tableId)
        {
            var table = await _db.RestaurantTables.FindAsync(tableId);
            if (table == null) return NotFound();

            var menuItems = await _db.MenuItems
                .Include(m => m.MenuCategory)
                .ToListAsync();

            if (!menuItems.Any())
            {
                // Seed menu if empty
                var cat1 = new MenuCategory { Name = "Mains" };
                var cat2 = new MenuCategory { Name = "Drinks" };
                var cat3 = new MenuCategory { Name = "Desserts" };
                _db.MenuCategories.AddRange(cat1, cat2, cat3);
                await _db.SaveChangesAsync();

                _db.MenuItems.AddRange(
                    new MenuItem { Name = "Beef Burger", Price = 89.99m, MenuCategoryId = cat1.Id },
                    new MenuItem { Name = "Grilled Chicken", Price = 99.99m, MenuCategoryId = cat1.Id },
                    new MenuItem { Name = "Margherita Pizza", Price = 109.99m, MenuCategoryId = cat1.Id },
                    new MenuItem { Name = "Caesar Salad", Price = 69.99m, MenuCategoryId = cat1.Id },
                    new MenuItem { Name = "Coke", Price = 24.99m, MenuCategoryId = cat2.Id },
                    new MenuItem { Name = "Still Water", Price = 14.99m, MenuCategoryId = cat2.Id },
                    new MenuItem { Name = "Orange Juice", Price = 29.99m, MenuCategoryId = cat2.Id },
                    new MenuItem { Name = "Chocolate Cake", Price = 49.99m, MenuCategoryId = cat3.Id },
                    new MenuItem { Name = "Ice Cream", Price = 39.99m, MenuCategoryId = cat3.Id }
                );
                await _db.SaveChangesAsync();
                menuItems = await _db.MenuItems.Include(m => m.MenuCategory).ToListAsync();
            }

            ViewBag.TableId = tableId;
            ViewBag.TableNumber = table.TableNumber;
            return View(menuItems);
        }

        // POST: /Waitron/SubmitOrder
        [HttpPost]
        public async Task<IActionResult> SubmitOrder(int tableId, List<int> menuItemIds, List<int> quantities, string? notes)
        {
            if (menuItemIds == null || !menuItemIds.Any())
                return RedirectToAction("CreateOrder", new { tableId });

            var menuItems = await _db.MenuItems.ToListAsync();

            var order = new Order
            {
                RestaurantTableId = tableId,
                WaitronId = _db.Users.First().Id,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                Notes = notes,
                OrderItems = new List<OrderItem>()
            };

            for (int i = 0; i < menuItemIds.Count; i++)
            {
                var item = menuItems.FirstOrDefault(m => m.Id == menuItemIds[i]);
                if (item != null && quantities[i] > 0)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        MenuItemId = item.Id,
                        Quantity = quantities[i],
                        Price = item.Price,
                        KitchenStatus = "Pending"
                    });
                }
            }

            if (!order.OrderItems.Any())
                return RedirectToAction("CreateOrder", new { tableId });

            // Mark table as occupied
            var table = await _db.RestaurantTables.FindAsync(tableId);
            if (table != null) table.Status = "Occupied";

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return RedirectToAction("MyTables");
        }

        // POST: /Waitron/PayOrder
        [HttpPost]
        public async Task<IActionResult> PayOrder(int orderId, string paymentMethod)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound();

            var total = order.OrderItems.Sum(i => i.Price * i.Quantity);

            _db.Payments.Add(new Payment
            {
                OrderId = orderId,
                TotalAmount = total,
                PaymentMethod = paymentMethod,
                PaidAt = DateTime.Now
            });

            order.Status = "Paid";

            var table = await _db.RestaurantTables.FindAsync(order.RestaurantTableId);
            if (table != null) table.Status = "Available";

            await _db.SaveChangesAsync();
            return RedirectToAction("Receipt", "Sales", new { id = orderId });
        }

        // GET: /Waitron/TableOrders?tableId=1
        public async Task<IActionResult> TableOrders(int tableId)
        {
            var table = await _db.RestaurantTables.FindAsync(tableId);
            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Payment)
                .Where(o => o.RestaurantTableId == tableId && o.Status != "Paid")
                .ToListAsync();

            ViewBag.TableNumber = table?.TableNumber;
            ViewBag.TableId = tableId;
            return View(orders);
        }
    }
}