namespace restaurant_pos_system.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime ReservationTime { get; set; }

        public int NumberOfGuests { get; set; }

        public int RestaurantTableId { get; set; }

        public RestaurantTable RestaurantTable { get; set; }
    }
}