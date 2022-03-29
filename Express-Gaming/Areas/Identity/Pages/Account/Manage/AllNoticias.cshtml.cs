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
using Microsoft.Extensions.Logging;

namespace Express_Gaming.Areas.Identity.Pages.Account.Manage
{
    public class AllNoticiasModel : PageModel
    {
        public ApplicationDbContext _context;
        public IList<Noticia> noticias { get; set; }
        public UserManager<Usuario> _userManager;

        [BindProperty]
        public int TotalRecords { get; set; }

        [BindProperty]
        public int PageNo { get; set; }

        [BindProperty]
        public int PageSize { get; set; }

        public AllNoticiasModel(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> OnGetAsync(int p = 1, int size = 5)
        {
            if (HttpContext.User.IsInRole("administrador") == true) 
            {
                IQueryable<Noticia> noticiasIQ = from s in _context.Noticias select s;

                PageNo = p;
                PageSize = size;

                var user = await _userManager.GetUserAsync(User);
                ViewData["UID"] = user.Id;

                TotalRecords = noticiasIQ.Count();

                noticias = await noticiasIQ.OrderByDescending(x => x.FechaPub).Where(x => x.UsuarioId == user.Id).Skip((p - 1) * size).Take(size).ToListAsync();

                return Page(); 
            }
            else
            {
                string url = "/Identity/Account/AccessDenied";
                return Redirect(url);
            }
        }
    }
}
