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
using System.Security.Claims;
using Unidad_IV.Helpers;

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
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            bool correoExiste = await db.Usuarios.AnyAsync(u => u.Correo.ToLower() == user.Correo.ToLower().Trim());

            if (correoExiste)
            {
                ModelState.AddModelError("Correo", "Este correo electrónico ya se encuentra registrado.");

                return View(user);
            }

            user.Correo = user.Correo.Trim();

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Usuarios>();
            user.Pwd = hasher.HashPassword(user, user.Pwd);

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
            var usuarioElegido = await db.Usuarios.FindAsync(usr.Id);
            if (usuarioElegido == null)
            {
                return NotFound();
            }
            bool correoExiste = await db.Usuarios.AnyAsync(u => u.Correo.ToLower() == usr.Correo.ToLower().Trim() && u.Id != usr.Id);
            if (correoExiste)
            {
                ModelState.AddModelError("Correo", "Este correo ya le pertenece a otro usuario.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IdRol = new SelectList(db.Roles, "Id", "Rol", usr.IdRol);
                return View(usr);
            }

            usuarioElegido.Nombre = usr.Nombre;
            usuarioElegido.Correo = usr.Correo.ToLower().Trim();
            usuarioElegido.IdRol = usr.IdRol;

            db.Entry(usuarioElegido).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(byte Id)
        {
            var user = await db.Usuarios.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            user.Estado = "Inactivo";
            db.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Principal");
        }

        [HttpGet]
        public IActionResult Perfil()
        {
            var userId = byte.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var usuario = db.Usuarios
        .Include(u => u.IdRolNavigation)
        .FirstOrDefault(u => u.Id == userId);

            var vm = new PerfilViewModel
            {
                Nombre = usuario!.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.IdRolNavigation.Rol
            };

            return View(vm);
        }

        public IActionResult CambiarPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = byte.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var usuario = db.Usuarios.Find(userId);

            var ps = new PasswordHelper();

            if (usuario == null)
            {
                return NotFound();
            }

            var valido = ps.VerifyPassword(
                usuario,
                usuario.Pwd,
                model.PasswordActual);

            if (!valido)
            {
                ModelState.AddModelError("", "Contraseña actual incorrecta");
                return View(model);
            }

            usuario.Pwd = ps.HashPassword(usuario, model.PasswordNueva);

            db.SaveChanges();
            return RedirectToAction("Perfil", "Usuarios");
        }
    }
}