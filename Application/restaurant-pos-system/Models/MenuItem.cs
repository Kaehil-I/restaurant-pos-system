namespace restaurant_pos_system.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int MenuCategoryId { get; set; }

        public MenuCategory MenuCategory { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public ICollection<MenuItemIngredient> Ingredients { get; set; }
    }
}