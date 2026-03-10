namespace restaurant_pos_system.Models
{
    public class MenuItemIngredient
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }

        public MenuItem MenuItem { get; set; }

        public int InventoryItemId { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public decimal QuantityRequired { get; set; }
    }
}