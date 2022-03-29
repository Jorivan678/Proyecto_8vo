﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Express_Gaming.Models
{
    public class Noticia
    {
        public int NoticiaId { get; set; }
        [Display(Name ="Título")]
        public string Titulo { get; set; }
        [Display(Name = "Fecha de publicación")]
        public DateTime FechaPub { get; set; } = DateTime.Now;
        [Display(Name ="Imagen")]
        public string Img { get; set; }
        [Display(Name ="Descripción")]
        public string Detalle { get; set; }
        public string Contenido { get; set; }

        [ForeignKey("Usuario")]
        [Display(Name = "Autor")]
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public ICollection<ComentarioNoticia> Comentarios { get; set; }

        [NotMapped]
        public IFormFile Foto { get; set; }
    }
}
