using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class Pagos
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateOnly FechaPago { get; set; }

    public decimal Monto { get; set; }

    public string? Descripcion { get; set; }

    public byte IdUsuario { get; set; }

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
