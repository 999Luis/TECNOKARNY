using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TECNOKARNY.Models;

namespace TECNOKARNY.Controllers
{
    public class InicioController : Controller
    {
        private readonly BdtecnokarnyContext db;

        public InicioController(BdtecnokarnyContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Bienvenida()
        {
            return View();
        }

        public IActionResult MenuInicio()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            var usuario = db.Usuarios.Include(x => x.IdRolNavigation)
                .FirstOrDefault(x => x.Correo == login.Usuario);

            if (usuario == null)
            {
                ViewBag.Error = "Usuario incorrecto";
                return View();
            }

            var hasher = new PasswordHasher<Usuarios>();
            var result = hasher.VerifyHashedPassword(
                usuario,
                usuario.Pwd,
                login.Contrasenia
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Contraseña incorrecta";
                return View();
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.IdRolNavigation.Rol)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("MenuInicio");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}