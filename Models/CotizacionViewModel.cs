using System.Collections.Generic;

namespace TECNOKARNY.Models
{
    public class CotizacionViewModel
    {
        public short? Id { get; set; }
        public DateOnly FechaEmision { get; set; }
        public DateOnly FechaEvento { get; set; }
        public TimeOnly Hora { get; set; }
        public string Direccion { get; set; } = null!;
        public short Asistentes { get; set; }
        public bool Anticipo { get; set; }
        public string EstadoCot { get; set; } = "Vigente";
        public decimal SaldoTotal { get; set; }
        public short IdCliente { get; set; }
        public byte IdUsuario { get; set; }

        public List<DetalleCotizacionViewModel> Detalles { get; set; } = new();
    }

    public class DetalleCotizacionViewModel
    {
        public short IdProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioKilo { get; set; }
        public decimal PrecioCotizado { get; set; }
    }
}
