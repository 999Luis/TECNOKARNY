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
    public class UsuariosController : Controller
    {
        private readonly BdtecnokarnyContext db;

        public UsuariosController(BdtecnokarnyContext db)
        {
            this.db = db;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Principal()
        {
            return View(await db.Usuarios.Include(x => x.IdRolNavigation).ToListAsync());
            
        }
        
        [Authorize(Roles = "Administrador")]
         public IActionResult Crear()
        {
            ViewBag.IdRol = new SelectList(db.Roles.ToList(), "Id", "Rol");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(Usuarios user)
        {
            db.Usuarios.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(byte Id)
        {
            Usuarios? usr = await db.Usuarios.FindAsync(Id);
            if (usr == null)
            {
                return NotFound();
            }
            ViewBag.IdRol = new SelectList(db.Roles, "Id", "Rol", usr.IdRol);
            return View(usr);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(Usuarios usr)
        {
            db.Update(usr);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpPost]
        public async Task<IActionResult> Elminar(byte Id)
        {
            var user = await db.Usuarios.FindAsync(Id);
            db.Remove(user!);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }
    }
} 