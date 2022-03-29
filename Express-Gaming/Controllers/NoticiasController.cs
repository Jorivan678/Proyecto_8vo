using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Express_Gaming.Data;
using Express_Gaming.Models;
using X.PagedList.Mvc.Core;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Express_Gaming.Controllers
{
    public class NoticiasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserManager<Usuario> _userManager;
        public IWebHostEnvironment HostEnvironment { get; }
        public NoticiasController(ApplicationDbContext context, UserManager<Usuario> userManager, IWebHostEnvironment webHostEnvrionment)
        {
            _context = context;
            _userManager = userManager;
            HostEnvironment = webHostEnvrionment;
        }

        // GET: Noticias
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            /*var applicationDbContext = _context.Noticias.Include(n => n.Usuario);
            var getawaiter = await applicationDbContext.ToListAsync();*/
            IQueryable<Noticia> noticiasIQ = from s in _context.Noticias.Include(x => x.Usuario) select s;

            pageSize = (pageSize ?? 5);
            page = (page ?? 1);
            ViewBag.PageSize = pageSize;

            return View(await noticiasIQ.OrderByDescending(x => x.FechaPub).ToPagedListAsync(page.Value, pageSize.Value));
        }

        // GET: Noticias/Create
        [HttpGet]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UID"] = user.Id;
            return View();
        }

        // POST: Noticias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Create([Bind("NoticiaId,Titulo,FechaPub,Img,Detalle,Contenido,UsuarioId,Foto")] Noticia noticia)
        {
            if (ModelState.IsValid)
            {
                if (noticia.Foto != null)
                {
                    if (!string.IsNullOrEmpty(noticia.Img))
                    {
                        var filePath = Path.Combine(HostEnvironment.WebRootPath, "img/posts", noticia.Img);
                        System.IO.File.Delete(filePath);
                    }
                    noticia.Img = ProcessUploadFile(noticia);
                }
                _context.Add(noticia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(noticia);
        }

        // GET: Noticias/Edit/5
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noticia = await _context.Noticias.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (noticia.UsuarioId == user.Id)
            {
                if (noticia == null)
                {
                    return NotFound();
                }
                return View(noticia);
            }
            else
            {
                string url = "/Identity/Account/AccessDenied";
                return Redirect(url);
            }
        }

        // POST: Noticias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("NoticiaId,Titulo,FechaPub,Img,Detalle,Contenido,UsuarioId,Foto")] Noticia noticia)
        {
            if (id != noticia.NoticiaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (noticia.Foto != null)
                    {
                        if (!string.IsNullOrEmpty(noticia.Img))
                        {
                            var filePath = Path.Combine(HostEnvironment.WebRootPath, "img/posts", noticia.Img);
                            System.IO.File.Delete(filePath);
                        }
                        noticia.Img = ProcessUploadFile(noticia);
                    }

                    _context.Update(noticia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticiaExists(noticia.NoticiaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(noticia);
        }

        // GET: Noticias/Delete/5
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noticia = await _context.Noticias
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.NoticiaId == id);
            var user = await _userManager.GetUserAsync(User);

            if (noticia == null)
            {
                return NotFound();
            }

            if (noticia.UsuarioId == user.Id)
            {
                return View(noticia);
            }
            else
            {
                string url = "/Identity/Account/AccessDenied";
                return Redirect(url);
            }
        }

        // POST: Noticias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noticia = await _context.Noticias.FindAsync(id);
            _context.Noticias.Remove(noticia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string ProcessUploadFile(Noticia noticia)
        {
            if (noticia.Foto == null)
                return string.Empty;

            var uploadFolder = Path.Combine(HostEnvironment.WebRootPath, "img/posts");
            var fileName = $"{Guid.NewGuid()}_{noticia.Foto.FileName}";
            var filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                noticia.Foto.CopyTo(stream);
            }
            return fileName;
        }

        private bool NoticiaExists(int id)
        {
            return _context.Noticias.Any(e => e.NoticiaId == id);
        }
    }
}
