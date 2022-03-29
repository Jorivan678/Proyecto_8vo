using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Express_Gaming.Models
{
    public class Usuario : IdentityUser
    {
        public ICollection<Noticia> Noticias { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ComentarioNoticia> CoNotis { get; set; }
        public ICollection<ComentarioReview> CoReviews { get; set; }
    }
}
