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
        public async Task<IActionResult> Principal()
        {
            var productos = await db.Productos.ToListAsync();
            return View(productos);
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Productos producto)
        {
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
        public async Task<IActionResult> Eliminar(short Id)
        {
            var producto = await db.Productos.FindAsync(Id);
            db.Remove(producto!);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }
    }
}