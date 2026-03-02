using Microsoft.AspNetCore.Identity;

namespace restaurant_pos_system.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Manager, Waiter, Kitchen
        public string RoleType { get; set; }

        // Hashed 4-digit PIN (we store the hashed PIN separately)
        public string PinHash { get; set; }
    }
}
    
