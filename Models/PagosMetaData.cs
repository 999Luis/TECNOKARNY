using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TECNOKARNY.Models
{
    [ModelMetadataType(typeof(PagosMetaData))]
    public partial class Pagos { }
    public class PagosMetaData
    {
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = null!;
    [Required(ErrorMessage = "El tipo de pago es obligatorio")]
    public string Tipo { get; set; } = null!;
    [Phone(ErrorMessage = "El número de teléfono es obligatorio")]
    public string? Telefono { get; set; }
    public DateOnly FechaPago { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero")]
    public decimal Monto { get; set; }
    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string? Descripcion { get; set; }
    public byte IdUsuario { get; set; }
    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
    }
}