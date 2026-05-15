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
    }
}