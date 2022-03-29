using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Express_Gaming.Models
{
    public class ComentarioReview
    {
        [Key]
        public int CoReId { get; set; }
        [Display(Name ="Comentario")]
        [Required]
        public string Mensaje { get; set; }
        [Display(Name ="Fecha de publicación")]
        public DateTime FechPub { get; set; } = DateTime.Now;

        [ForeignKey("Review")]
        public int ReviewId { get; set; }
        public Review Review { get; set; }

        [ForeignKey("Usuario")]
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
