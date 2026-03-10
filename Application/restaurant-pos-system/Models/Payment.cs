namespace restaurant_pos_system.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } // Cash, Card

        public DateTime PaidAt { get; set; }
    }
}