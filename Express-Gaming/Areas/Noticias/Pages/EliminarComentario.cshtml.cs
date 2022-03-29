using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Express_Gaming.Data;
using Express_Gaming.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Express_Gaming.Areas.Noticias.Pages
{
    public class EliminarCModel : PageModel
    {
        [BindProperty]
        public ComentarioNoticia Comentario { get; set; }
        public readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        public IEnumerable<Usuario> usuarios { get; set; }
        public EliminarCModel(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? ConoID, int? NoticiaID)
        {
            if (ConoID == null || NoticiaID == null)
            {
                return NotFound();
            }

            Comentario = await _context.ComentariosNoticias.Include(c => c.Noticia)
                .FirstOrDefaultAsync(m => m.CoNoId == ConoID && m.NoticiasId == NoticiaID);

            var uid = await _userManager.GetUserAsync(User);

            try
            {
                if (Comentario.UsuarioId == uid.Id || HttpContext.User.IsInRole("administrador") == true)
                {
                    if (Comentario == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }
                else
                {
                    string url = "/Identity/Account/AccessDenied";
                    return Redirect(url);
                }
            }
            catch(Exception e)
            {
                return NotFound();
            }
        }


        public async Task<IActionResult> OnPostAsync(int? ConoID, int? NoticiaID)
        {
            if (ConoID == null || NoticiaID == null)
            {
                return NotFound();
            }

            Comentario = await _context.ComentariosNoticias.FindAsync(ConoID);

            if (Comentario != null)
            {
                _context.ComentariosNoticias.Remove(Comentario);
                await _context.SaveChangesAsync();
            }


            string url = "/Noticias/Articulo?id=" + NoticiaID;
            return Redirect(url);
        }
    }
}
