namespace restaurant_pos_system.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; } // g, ml, pcs

        public decimal ReorderLevel { get; set; }

        public ICollection<MenuItemIngredient> MenuItems { get; set; }
    }
}