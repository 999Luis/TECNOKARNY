using System;
using System.Collections.Generic;
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
    public class ClientesController : Controller
    {
        private readonly BdtecnokarnyContext db;

        public ClientesController(BdtecnokarnyContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> Principal()
        {
            var clientes = await db.Clientes.ToListAsync();
            return View(clientes);
            
        }
         public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(Clientes cliente)
        {
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpGet]
        public async Task<IActionResult> Actualizar(short Id)
        {
            Clientes? c = await db.Clientes.FindAsync(Id);
            if (c == null)
            {
                return NotFound();
            }
            return View(c);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(Clientes cliente)
        {
            db.Update(cliente);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpPost]
        public async Task<IActionResult>Eliminar(short Id)
        {
            var cliente = await db.Clientes.FindAsync(Id);
            db.Remove(cliente!);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }
    }
} 