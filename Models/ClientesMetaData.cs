using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TECNOKARNY.Models
{
    [ModelMetadataType(typeof(ClientesMetaData))]
    public partial class Clientes { }
    public class ClientesMetaData
    {
        public short Id { get; set; }

        [Display(Name = "Nombre(s) del cliente")]
        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Apellido Paterno")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string ApePat { get; set; } = null!;

        [Display(Name = "Apellido Materno")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string ApeMat { get; set; } = null!;

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Telefono { get; set; } = null!;

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Correo { get; set; } = null!;

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }
    }
}