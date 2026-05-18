using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class Productos
{
    public short Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PrecioKilo { get; set; }
    public string? Estado { get; set; }

    public virtual ICollection<DetalleCotizacion> DetalleCotizacion { get; set; } = new List<DetalleCotizacion>();

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();
}
