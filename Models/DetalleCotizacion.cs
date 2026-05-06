using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class DetalleCotizacion
{
    public int Id { get; set; }

    public decimal Cantidad { get; set; }

    public decimal PrecioKilo { get; set; }

    public decimal PrecioCotizado { get; set; }

    public short IdCotizacion { get; set; }

    public short IdProducto { get; set; }

    public virtual Cotizaciones IdCotizacionNavigation { get; set; } = null!;

    public virtual Productos IdProductoNavigation { get; set; } = null!;
}
