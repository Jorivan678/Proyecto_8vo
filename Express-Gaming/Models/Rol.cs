using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Express_Gaming.Models
{
    public class Rol : IdentityRole
    {
        public string Descripcion { get; set; }
    }
}
