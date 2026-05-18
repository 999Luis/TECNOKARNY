using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TECNOKARNY.Models;

namespace TECNOKARNY.Controllers
{
    public class ProductosController : Controller
    {
        private readonly BdtecnokarnyContext db;
        public ProductosController(BdtecnokarnyContext db)
        {
            this.db = db;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Principal(string? busqueda)
        {
            ViewBag.Busqueda = busqueda;
            var consulta = db.Productos
                .Where(p => p.Estado != "Inactivo")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                consulta = consulta.Where(p => p.Nombre.Contains(busqueda));
            }
            return View(await consulta.ToListAsync());
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear(Productos producto)
        {
            var existe = await db.Productos.AnyAsync(p => p.Nombre == producto.Nombre && p.Estado != "Inactivo");
            if (existe)
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con ese nombre.");
                return View(producto);
            }

            producto.Estado = "Activo";
            db.Productos.Add(producto);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(short Id)
        {
            var producto = await db.Productos.FindAsync(Id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
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

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(short Id)
        {
            var producto = await db.Productos.FindAsync(Id);
            if (producto == null) return NotFound();

            producto.Estado = "Inactivo";
            db.Update(producto);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }
    }
}