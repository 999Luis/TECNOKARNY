using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class Clientes
{
    public short Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string ApePat { get; set; } = null!;

    public string ApeMat { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Direccion { get; set; }

    public virtual ICollection<Cotizaciones> Cotizaciones { get; set; } = new List<Cotizaciones>();

    public virtual ICollection<Ventas> Ventas { get; set; } = new List<Ventas>();
}
