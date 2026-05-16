using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TECNOKARNY.Models;

namespace TECNOKARNY.Controllers
{
    public class PagosController : Controller
    {
        private readonly BdtecnokarnyContext db;
        public PagosController(BdtecnokarnyContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> Principal(string? busqueda)
        {
            ViewBag.Busqueda = busqueda;
            var consulta = db.Pagos.Include(x => x.IdUsuarioNavigation).AsQueryable();
            if (!User.IsInRole("Administrador"))
            {
                var UserId = byte.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                consulta = consulta.Where(p => p.IdUsuario == UserId);
            }
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                consulta = consulta.Where(p => p.IdUsuarioNavigation.Nombre.Contains(busqueda));
            }
            var pagos = await consulta.OrderByDescending(p => p.FechaPago).ToListAsync();
            return View(pagos);
        }
        public async Task<IActionResult> Crear()
        {
            ViewBag.IdUsuario = new SelectList(db.Usuarios.ToList(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Pagos pago)
        {
            db.Pagos.Add(pago);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpGet]
        public async Task<IActionResult> Actualizar(int Id)
        {
            var pago = await db.Pagos.FindAsync(Id);
            if (pago == null) return NotFound();
            ViewBag.IdUsuario = new SelectList(db.Usuarios, "Id", "Nombre", pago.IdUsuario);
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(Productos producto)
        {
            if (ModelState.IsValid)
            {
                db.Update(producto);
                await db.SaveChangesAsync();
                return RedirectToAction("Principal");
            }

            return View(producto);
        }
        [HttpGet]
        public IActionResult Informe()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Informe(DateOnly fechaInicio, DateOnly fechaFin)
        {
            if (fechaInicio > fechaFin)
            {
                ViewBag.Error = "La fecha de inicio no puede ser mayor a la fecha fin.";
                return View();
            }

            if (fechaFin > DateOnly.FromDateTime(DateTime.Now))
            {
                ViewBag.Error = "La fecha fin no puede ser mayor a la fecha actual.";
                return View();
            }

            var ventas = await db.Ventas
                .Where(v => v.Fecha >= fechaInicio
                         && v.Fecha <= fechaFin
                         && v.Estado != "Cancelada"
                         && v.Tipo == "Contado")
                .ToListAsync();

            var pagos = await db.Pagos
                .Where(p => p.FechaPago >= fechaInicio && p.FechaPago <= fechaFin)
                .ToListAsync();

            var totalIngresos = ventas.Sum(v => v.MontoTotal);
            var totalEgresos = pagos.Sum(p => p.Monto);

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            ViewBag.TotalIngresos = totalIngresos;
            ViewBag.TotalEgresos = totalEgresos;
            ViewBag.Balance = totalIngresos - totalEgresos;
            ViewBag.Ventas = ventas;
            ViewBag.Pagos = pagos;

            return View();
        }
    }
}