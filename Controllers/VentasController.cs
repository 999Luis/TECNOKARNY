using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TECNOKARNY.Models;

namespace TECNOKARNY.Controllers
{
    public class VentasController : Controller
    {
        private readonly BdtecnokarnyContext db;

        public VentasController(BdtecnokarnyContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Principal(string? busqueda)
        {
            ViewBag.Busqueda = busqueda;

            var consulta = db.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .Include(v => v.DetalleVenta)
                .AsQueryable();
                if (!User.IsInRole("Administrador"))
            {
                var UserId = byte.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                consulta = consulta.Where(v => v.IdUsuario == UserId);
            }

            if (!string.IsNullOrWhiteSpace(busqueda)){
                consulta = consulta.Where(v => v.IdClienteNavigation.Nombre.Contains(busqueda));
            }
            var ventas = await consulta.OrderByDescending(v => v.Fecha).ToListAsync();

            return View(ventas);
        }
        public async Task<IActionResult> Crear()
        {
            await CargarListasAsync();
            return View(new VentaViewModel { Fecha = DateOnly.FromDateTime(DateTime.Now), FechaVencimiento = DateOnly.FromDateTime(DateTime.Now.AddDays(7)) });
        }

        [HttpPost]
        public async Task<IActionResult> Crear(VentaViewModel ventaVM)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(ventaVM);
            }

            if (ventaVM.DetallesVenta == null || ventaVM.DetallesVenta.Count == 0)
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto a la venta.");
                await CargarListasAsync();
                return View(ventaVM);
            }

            if (ventaVM.Fecha == default)
            {
                ventaVM.Fecha = DateOnly.FromDateTime(DateTime.Now);
            }

            if (string.IsNullOrWhiteSpace(ventaVM.Tipo))
            {
                ModelState.AddModelError("Tipo", "El tipo de venta es obligatorio.");
                await CargarListasAsync();
                return View(ventaVM);
            }

            var venta = new Ventas
            {
                IdCliente = ventaVM.IdCliente,
                IdUsuario = ventaVM.IdUsuario,
                Fecha = ventaVM.Fecha,
                Tipo = ventaVM.Tipo,
                MontoTotal = ventaVM.MontoTotal,
                Estado = "Activa",
                MotivoCancelacion = null,
                FechaVencimiento = ventaVM.FechaVencimiento
            };

            if (venta.Tipo.Trim().Equals("Contado", StringComparison.OrdinalIgnoreCase))
            {
                venta.Saldo = 0m;
            }
            else
            {
                venta.Saldo = ventaVM.Saldo > 0m ? ventaVM.Saldo : ventaVM.MontoTotal;
            }

            db.Ventas.Add(venta);
            await db.SaveChangesAsync();

            // Guardar detalles de venta
            foreach (var detalle in ventaVM.DetallesVenta)
            {
                var detalleVenta = new DetalleVenta
                {
                    IdVenta = venta.Id,
                    IdProducto = detalle.IdProducto,
                    Cantidad = detalle.Cantidad,
                    PrecioKilo = detalle.PrecioKilo,
                    Subtotal = detalle.Subtotal
                };
                db.DetalleVenta.Add(detalleVenta);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarDeuda(int idCliente)
        {
            var ventaPendiente = await db.Ventas
                .Where(v => v.IdCliente == idCliente && v.Saldo > 0)
                .Select(v => new { v.Id, v.Saldo })
                .FirstOrDefaultAsync();

            return Json(ventaPendiente);
        }

        private async Task CargarListasAsync()
        {
            var clientes = await db.Clientes
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.ApePat)
                .ToListAsync();
            var usuarios = await db.Usuarios
                .OrderBy(u => u.Nombre)
                .ToListAsync();
            var productos = await db.Productos
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre");
            ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nombre");
            ViewBag.Productos = new SelectList(productos, "Id", "Nombre");
            ViewBag.ProductosJSON = System.Text.Json.JsonSerializer.Serialize(
                productos.Select(p => new { p.Id, p.Nombre, p.PrecioKilo }).ToList()
            );
        }

        public async Task<IActionResult> Actualizar(int id)
        {
            var ventaActualizar = await db.Ventas
                .Include(v => v.DetalleVenta)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (ventaActualizar == null)
            {
                return NotFound();
            }
            var ventaVM = new VentaViewModel

            {
                Id = ventaActualizar.Id,
                IdCliente = ventaActualizar.IdCliente,
                IdUsuario = ventaActualizar.IdUsuario,
                Fecha = ventaActualizar.Fecha,
                Tipo = ventaActualizar.Tipo,
                MontoTotal = ventaActualizar.MontoTotal,
                Estado = ventaActualizar.Estado,
                MotivoCancelacion = ventaActualizar.MotivoCancelacion,
                Saldo = ventaActualizar.Saldo,
                FechaVencimiento = ventaActualizar.FechaVencimiento,
                DetallesVenta = ventaActualizar.DetalleVenta.Select(d => new DetalleVentaViewModel
                {
                    IdProducto = d.IdProducto,
                    Cantidad = d.Cantidad,
                    PrecioKilo = d.PrecioKilo,
                    Subtotal = d.Subtotal
                }).ToList()
            };
            await CargarListasAsync();
            return View(ventaVM);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(VentaViewModel ventaVM)
        {
            var ventaModificar = await db.Ventas
                .Include(v => v.DetalleVenta)
                .FirstOrDefaultAsync(ventaModificar => ventaModificar.Id == ventaVM.Id);

            if (ventaModificar == null)
            {
                return NotFound();
            }
            ventaModificar.IdCliente = ventaVM.IdCliente;
            ventaModificar.IdUsuario = ventaVM.IdUsuario;
            ventaModificar.MontoTotal = ventaVM.MontoTotal;
            ventaModificar.Tipo = ventaVM.Tipo;
            ventaModificar.Saldo = ventaVM.Tipo == "Contado" ? 0m : ventaVM.MontoTotal;
            ventaModificar.FechaVencimiento = ventaVM.Tipo == "Crédito" ? ventaVM.FechaVencimiento: null;
            foreach (var detalleVM in ventaVM.DetallesVenta)
            {
                var detalleGuardado = ventaModificar.DetalleVenta
                    .FirstOrDefault(det => det.IdProducto == detalleVM.IdProducto);

                if (detalleGuardado != null)
                {
                    detalleGuardado.Cantidad = detalleVM.Cantidad;
                    detalleGuardado.Subtotal = detalleVM.Cantidad * detalleVM.PrecioKilo;
                }
            }
            db.Update(ventaModificar);
            await db.SaveChangesAsync();

            return RedirectToAction("Principal");
        }

        [HttpPost]
        public async Task<IActionResult> Pagar (int id)
        {
            var venta = await db.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            venta.Saldo = 0m;
            venta.Tipo = "Contado";
            db.Update(venta);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpPost]
        public async Task<IActionResult> Cancelar (int id, string motivoCancelacion)
        {
            var venta = await db.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            venta.Estado = "Cancelada";
            venta.Saldo =0m;
            venta.MotivoCancelacion = motivoCancelacion;
            db.Update(venta);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        public async Task<IActionResult> DetalleVenta(int id, int? idCliente)
        {
            var venta = await db.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(v => v.Id == id);



            if (venta == null)
            {
                return NotFound();
            }

            ViewBag.IdCliente = idCliente;
            
            return View(venta);
        }

        public async Task<IActionResult> TotalVentas()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TotalVentas(DateOnly fecha)
        {
            if (fecha > DateOnly.FromDateTime(DateTime.Now))
            {
                ViewBag.Error = "La fecha no puede ser mayor al dia de hoy.";
                return View();
            }

            var ventas = await db.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.DetalleVenta)
                .ThenInclude(d => d.IdProductoNavigation)
                .Where(v => v.Fecha == fecha && v.Estado != "Cancelada")
                .ToListAsync();

                if (!ventas.Any())
                {
                    ViewBag.Fecha = fecha;
                    ViewBag.SinVentas = true;
                    return View();
                }

                ViewBag.Fecha = fecha;
                ViewBag.TotalVentas = ventas.Sum(v => v.MontoTotal);
                ViewBag.TotalIngresos = ventas.Where(v => v.Tipo.Trim() == "Contado").Sum(v => v.MontoTotal);
                ViewBag.TotalCredito = ventas.Where(v => v.Tipo.Trim() == "Crédito").Sum(v => v.MontoTotal);
                ViewBag.Ventas = ventas;

                return View();
        }
    }
}
