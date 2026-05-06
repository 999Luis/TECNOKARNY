using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class Cotizaciones
{
    public short Id { get; set; }

    public DateOnly FechaEmision { get; set; }

    public DateOnly FechaEvento { get; set; }

    public TimeOnly Hora { get; set; }

    public string Direccion { get; set; } = null!;

    public short Asistentes { get; set; }

    public bool? Anticipo { get; set; }

    public string EstadoCot { get; set; } = null!;

    public decimal SaldoTotal { get; set; }

    public short IdCliente { get; set; }

    public byte IdUsuario { get; set; }

    public virtual ICollection<DetalleCotizacion> DetalleCotizacion { get; set; } = new List<DetalleCotizacion>();

    public virtual Clientes IdClienteNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
