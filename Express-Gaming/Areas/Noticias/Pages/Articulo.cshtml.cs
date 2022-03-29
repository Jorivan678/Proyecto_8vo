using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Express_Gaming.Data;
using Express_Gaming.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Express_Gaming.Areas.Noticias.Pages
{
    [AllowAnonymous]
    public class ArticuloModel : PageModel
    {
        public Noticia noticia { get; set; }
        [BindProperty]
        public ComentarioNoticia Comentario { get; set; }
        public IEnumerable<Usuario> usuarios { get; set; }
        private readonly UserManager<Usuario> _userManager;
        private readonly ApplicationDbContext _context;
        public ArticuloModel(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["RID"] = id;

            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                var uid = await _userManager.GetUserAsync(User);
                ViewData["UID"] = uid.Id;
            }

            noticia = await _context.Noticias
                .Include(r => r.Usuario)
                .Include(r => r.Comentarios)
                .FirstOrDefaultAsync(m => m.NoticiaId == id);

            usuarios = _context.Users.ToList();

            if (noticia == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([Bind("CoNoId,Mensaje,FechPub,NoticiasId,UsuarioId")]int? id)
        {
            if (HttpContext.User.Identity.IsAuthenticated != true)
            {
                string url = "/Identity/Account/Login";
                return Redirect(url);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                _context.ComentariosNoticias.Add(Comentario);
                await _context.SaveChangesAsync();

                if(id == null)
                {
                    return NotFound();
                }

                string url = "/Noticias/Articulo?id=" + id;
                return Redirect(url);
            }
        }
    }
}
