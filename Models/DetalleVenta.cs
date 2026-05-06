using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class DetalleVenta
{
    public int Id { get; set; }

    public decimal Cantidad { get; set; }

    public decimal PrecioKilo { get; set; }

    public decimal Subtotal { get; set; }

    public int IdVenta { get; set; }

    public short IdProducto { get; set; }

    public virtual Productos IdProductoNavigation { get; set; } = null!;

    public virtual Ventas IdVentaNavigation { get; set; } = null!;
}
