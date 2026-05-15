using System.Collections.Generic;

namespace TECNOKARNY.Models
{
    public class VentaViewModel
    {
        public int? Id { get; set; }
        public decimal MontoTotal { get; set; }
        public DateOnly Fecha { get; set; }
        public string? Estado { get; set; }
        public string? MotivoCancelacion { get; set; }
        public string Tipo { get; set; } = null!;
        public decimal Saldo { get; set; }
        public DateOnly? FechaVencimiento { get; set; }
        public short IdCliente { get; set; }
        public byte IdUsuario { get; set; }

        public List<DetalleVentaViewModel> DetallesVenta { get; set; } = new List<DetalleVentaViewModel>();
    }

    public class DetalleVentaViewModel
    {
        public short IdProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioKilo { get; set; }
        public decimal Subtotal { get; set; }
    }
}
