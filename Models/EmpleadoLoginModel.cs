using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class EmpleadoLoginModel
    {

        [Required]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        public bool Recordar { get; set; }
    }
}
