using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using TECNOKARNY.Models;

namespace TECNOKARNY.Controllers
{
    [Authorize]
    public class CotizacionesController : Controller
    {
        private readonly BdtecnokarnyContext db;

        public CotizacionesController(BdtecnokarnyContext db)
        {
            this.db = db;
        }


        public async Task<IActionResult> Principal(string? buscar)
        {
            ViewBag.Buscar = buscar;

            var query = db.Cotizaciones
                .Include(c => c.IdClienteNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .AsQueryable();

            if (!User.IsInRole("Administrador"))
            {
                var userId = byte.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                query = query.Where(c => c.IdUsuario == userId);
            }

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(c => c.IdClienteNavigation.Nombre.Contains(buscar));

            var cotizaciones = await query
                .OrderByDescending(c => c.FechaEmision)
                .ToListAsync();

            return View(cotizaciones);
        }

        public IActionResult Crear()
        {
            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "Id", "Nombre");
            var productos = db.Productos.ToList();
            ViewBag.ProductosJSON = JsonSerializer.Serialize(productos.Select(p => new
            {
                p.Id,
                p.Nombre,
                p.PrecioKilo
            }));
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerificarFecha(string fecha)
        {
            if (!DateOnly.TryParse(fecha, out var fechaEvento))
                return Json(new { ocupada = false });

            var ocupada = await db.Cotizaciones
                .AnyAsync(c => c.FechaEvento == fechaEvento
                            && c.EstadoCot != "Expirada"
                            && c.EstadoCot != "Finalizada");

            return Json(new { ocupada });
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CotizacionViewModel vm)
        {
            var fechaOcupada = await db.Cotizaciones
                .AnyAsync(c => c.FechaEvento == vm.FechaEvento
                            && c.EstadoCot != "Expirada"
                            && c.EstadoCot != "Finalizada");

            if (fechaOcupada)
                ModelState.AddModelError("FechaEvento", "Ya existe una cotización activa para esa fecha.");

            if (vm.Detalles == null || vm.Detalles.Count == 0)
                ModelState.AddModelError("", "Debe agregar al menos un producto.");

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "Id", "Nombre");
                var prods = db.Productos.ToList();
                ViewBag.ProductosJSON = JsonSerializer.Serialize(prods.Select(p => new { p.Id, p.Nombre, p.PrecioKilo }));
                return View(vm);
            }

            var userId = byte.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cotizacion = new Cotizaciones
            {
                FechaEmision = DateOnly.FromDateTime(DateTime.Now),
                FechaEvento = vm.FechaEvento,
                Hora = vm.Hora,
                Direccion = vm.Direccion,
                Asistentes = vm.Asistentes,
                Anticipo = vm.Anticipo,
                EstadoCot = "Vigente",
                SaldoTotal = vm.Detalles.Sum(d => d.PrecioCotizado),
                IdCliente = vm.IdCliente,
                IdUsuario = userId
            };

            db.Cotizaciones.Add(cotizacion);
            await db.SaveChangesAsync();

            foreach (var detalle in vm.Detalles)
            {
                db.DetalleCotizacion.Add(new DetalleCotizacion
                {
                    IdCotizacion = cotizacion.Id,
                    IdProducto = detalle.IdProducto,
                    Cantidad = detalle.Cantidad,
                    PrecioKilo = detalle.PrecioKilo,
                    PrecioCotizado = detalle.PrecioCotizado
                });
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        public async Task<IActionResult> DetalleCotizacion(short id)
        {
            var cotizacion = await db.Cotizaciones
                .Include(c => c.IdClienteNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.DetalleCotizacion)
                    .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cotizacion == null) return NotFound();

            return View(cotizacion);
        }

        // POST: /Cotizaciones/CambiarEstado
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(short id, string estado)
        {
            var cotizacion = await db.Cotizaciones.FindAsync(id);
            if (cotizacion == null) return NotFound();

            cotizacion.EstadoCot = estado;
            db.Update(cotizacion);
            await db.SaveChangesAsync();

            return RedirectToAction("Detalle", new { id });
        }
        public async Task<IActionResult> CotizacionesCliente(short id)
        {
            var cliente = await db.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            var cotizaciones = await db.Cotizaciones
                .Include(c => c.IdClienteNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Where(c => c.IdCliente == id)
                .OrderByDescending(c => c.FechaEmision)
                .ToListAsync();

            ViewBag.NombreCliente = $"{cliente.Nombre} {cliente.ApePat} {cliente.ApeMat}";
            return View("Principal", cotizaciones);
        }
    }
}