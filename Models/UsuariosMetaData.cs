using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TECNOKARNY.Models
{
    [ModelMetadataType(typeof(UsuariosMetaData))]
    public partial class Usuarios { }
    public class UsuariosMetaData
    {
        public short Id { get; set; }
        [Display(Name = "Nombre del usuario")]
        [Required(ErrorMessage = "El nombre del usuario es obligatorio")]
        public string Nombre { get; set; } = null!;
        [Display(Name = "Correo del usuario")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        public string Correo { get; set; } = null!;
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$", 
        ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula y un símbolo.")]
        public string Pwd { get; set; } = null!;
        [Display(Name = "Rol del usuario")]
        [Required(ErrorMessage = "El rol es obligatorio")]
        public byte IdRol { get; set; }
    }
}