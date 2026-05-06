using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class Ventas
{
    public int Id { get; set; }

    public decimal MontoTotal { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Estado { get; set; }

    public string? MotivoCancelacion { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Saldo { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public short IdCliente { get; set; }

    public byte IdUsuario { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Clientes IdClienteNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
