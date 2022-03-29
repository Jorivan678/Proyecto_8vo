using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Express_Gaming.Models
{
    public class Contacto
    {
        [Key]
        public int ContactoId { get; set; }
        [Display(Name ="Nombre(s)")]
        public string Nombre { get; set; }
        [Display(Name ="Apellido(s)")]
        public string Apellido { get; set; }
        [Display(Name ="Número de teléfono")]
        public string NumTel { get; set; }
        public string Correo { get; set; }
        public string Mensaje { get; set; }
    }
}
