namespace restaurant_pos_system.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string Action { get; set; }

        public DateTime Timestamp { get; set; }
    }
}