namespace restaurant_pos_system.Models

using System.ComponentModel.DataAnnotations;

namespace YourProjectName.Models
{
    public class Manager
    {
        [Key]
        [Required]
        public int ManagerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; }
    }
}