using System;
using System.Collections.Generic;

namespace TECNOKARNY.Models;

public partial class Roles
{
    public byte Id { get; set; }

    public string Rol { get; set; } = null!;

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}
