using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class Usuarios
{
    public byte Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Pwd { get; set; } = null!;

    public string? Estado { get; set; }

    public byte IdRol { get; set; }

    public virtual ICollection<Cotizaciones> Cotizaciones { get; set; } = new List<Cotizaciones>();

    public virtual Roles IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Pagos> Pagos { get; set; } = new List<Pagos>();

    public virtual ICollection<Ventas> Ventas { get; set; } = new List<Ventas>();
}
