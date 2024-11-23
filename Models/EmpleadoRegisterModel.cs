﻿using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public enum UserRole
    {

    }

    public class EmpleadoRegisterModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(256)]
        public string Usuario { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; }

        [Required]
        public string Rol { get; set; }
    }
}