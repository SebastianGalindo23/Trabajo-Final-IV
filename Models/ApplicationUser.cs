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
    }
}
