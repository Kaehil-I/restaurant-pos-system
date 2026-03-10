namespace restaurant_pos_system.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int MenuItemId { get; set; }

        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string KitchenStatus { get; set; } // Pending, Cooking, Ready
    }
}