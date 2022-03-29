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

namespace Express_Gaming.Areas.Reviews.Pages
{
    public class EliminarCModel : PageModel
    {
        [BindProperty]
        public ComentarioReview Comentario { get; set; }
        public readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        public IEnumerable<Usuario> usuarios { get; set; }
        public EliminarCModel(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? CoreID, int? ReviewID)
        {
            if (CoreID == null || ReviewID == null)
            {
                return NotFound();
            }

            Comentario = await _context.ComentariosReview.Include(c => c.Review)
                .FirstOrDefaultAsync(m => m.CoReId == CoreID && m.ReviewId == ReviewID);

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


        public async Task<IActionResult> OnPostAsync(int? CoreID, int? ReviewID)
        {
            if (CoreID == null || ReviewID == null)
            {
                return NotFound();
            }

            Comentario = await _context.ComentariosReview.FindAsync(CoreID);
            var uid = await _userManager.GetUserAsync(User);

            if (Comentario.UsuarioId == uid.Id || HttpContext.User.IsInRole("administrador") == true)
            {

                if (Comentario != null)
                {
                    _context.ComentariosReview.Remove(Comentario);
                    await _context.SaveChangesAsync();
                }

                string url = "/Reviews/Articulo?id=" + ReviewID;
                return Redirect(url);
            }
            else
            {
                string url = "/Identity/Account/AccessDenied";
                return Redirect(url);
            }
        }
    }
}
