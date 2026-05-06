namespace restaurant_pos_system.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int RestaurantTableId { get; set; }

        public RestaurantTable RestaurantTable { get; set; }

        public string WaitronId { get; set; }

        public ApplicationUser Waitron { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public string? Notes { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public Payment Payment { get; set; }
    }
}