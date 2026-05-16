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

        public async Task<IActionResult> Principal(string? busqueda)
        {
            ViewBag.Busqueda = busqueda;

            var consulta = db.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();
                consulta = consulta.Where(c => c.Nombre.Contains(busqueda) || c.ApePat.Contains(busqueda) || c.ApeMat.Contains(busqueda));
            }

            var clientes = await consulta.OrderBy(c => c.Nombre).ToListAsync();

            return View(clientes);
        }
        
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(Clientes cliente)
        {
            bool correoExiste = await db.Clientes.AnyAsync(u => u.Correo.ToLower() == cliente.Correo.ToLower().Trim());

            if (correoExiste)
            {
                ModelState.AddModelError("Correo", "Este correo electrónico ya se encuentra registrado.");

                return View(cliente);
            }

            cliente.Correo = cliente.Correo.Trim();

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
            var clienteElegido = await db.Clientes.FindAsync(cliente.Id);
            if (clienteElegido == null)
            {
                return NotFound();
            }
            bool correoExiste = await db.Clientes.AnyAsync(c => c.Correo.ToLower() == cliente.Correo.ToLower().Trim() && c.Id != cliente.Id);
            if (correoExiste)
            {
                ModelState.AddModelError("Correo", "Este correo ya le pertenece a otro usuario.");
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            clienteElegido.Nombre = cliente.Nombre;
            clienteElegido.Correo = cliente.Correo.ToLower().Trim();

            db.Entry(clienteElegido).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(short Id)
        {
            var cliente = await db.Clientes.FindAsync(Id);
            db.Remove(cliente!);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        public async Task<IActionResult> VentasCliente(short Id)
        {
            var cliente = await db.Clientes.FindAsync(Id);
            if (cliente == null)
            {
                return NotFound();
            }
            var ventas = await db.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .Include(v => v.DetalleVenta)
                .Where(v => v.IdCliente == Id)
                .ToListAsync();

            ViewBag.NombreCliente = $"{cliente.Nombre} {cliente.ApePat} {cliente.ApeMat}";
            return View("~/Views/Ventas/Principal.cshtml", ventas);
        }

        public async Task<IActionResult> DetalleVenta(int id)
        {
            return RedirectToAction("DetalleVenta", "Ventas", new { id });
        }
    }
}