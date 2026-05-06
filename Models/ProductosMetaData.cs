using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TECNOKARNY.Models
{
    [ModelMetadataType(typeof(ProductosMetaData))]
    public partial class Productos { }
    public class ProductosMetaData
    {
        public short Id { get; set; }
        [Display(Name = "Nombre del producto")]
        [Required(ErrorMessage = "El nombre del prosucto es obligatorio")]
        public string Nombre { get; set; } = null!;
        [Display(Name = "Precio por Kilogramo")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
        public string PrecioKilo { get; set; } = null!;
    }
}