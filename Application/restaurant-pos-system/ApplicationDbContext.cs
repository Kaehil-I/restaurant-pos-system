using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace restaurant_pos_system.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RestaurantTable> RestaurantTables { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<InventoryItem> InventoryItems { get; set; }

        public DbSet<MenuItemIngredient> MenuItemIngredients { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}