using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TECNOKARNY.Models
{
    public class Login
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = null!;
        public string Contrasenia { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }
}