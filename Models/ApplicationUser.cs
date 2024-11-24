using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Rol { get; set; }

        [Required]
        [StringLength(13, ErrorMessage = "El DPI debe tener 13 dígitos.")]
        public string? DPI { get; set; }
    }
}
