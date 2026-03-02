using System.ComponentModel.DataAnnotations;

namespace restaurant_pos_system.Models
{
    public class AccountViewModel
    {
        [Required]
        public string Role { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string Pin { get; set; }
    }
}