using Microsoft.AspNetCore.Identity;

namespace restaurant_pos_system.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string RoleType { get; set; } // Manager, Waitron, Kitchen

        public string PinHash { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}