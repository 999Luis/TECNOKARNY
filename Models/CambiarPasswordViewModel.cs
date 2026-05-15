using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TECNOKARNY.Models
{
    public class CambiarPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string PasswordActual { get; set; } = null!;
 
        [Required]
        [DataType(DataType.Password)]
        public string PasswordNueva { get; set; } = null!;
 
        [Required]
        [Compare("PasswordNueva",
            ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; } = null!;
    }
}