namespace restaurant_pos_system.Models
{
    public class RestaurantTable
    {
        public int Id { get; set; }

        public int TableNumber { get; set; }

        public int Capacity { get; set; }

        public string Status { get; set; } // Available, Occupied, Reserved

        public ICollection<Order> Orders { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}