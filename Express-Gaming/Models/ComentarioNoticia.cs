using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Express_Gaming.Models
{
    public class ComentarioNoticia
    {
        [Key]
        public int CoNoId { get; set; }
        [Display(Name ="Comentario")]
        public string Mensaje { get; set; }
        [Display(Name ="Fecha de publicación")]
        public DateTime FechPub { get; set; } = DateTime.Now;

        [ForeignKey("Noticia")]
        public int NoticiasId { get; set; }
        public Noticia Noticia { get; set; }

        [ForeignKey("Usuario")]
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
