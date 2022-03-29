using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Express_Gaming.Data;
using Express_Gaming.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Mvc.Core;
using X.PagedList;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Express_Gaming.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserManager<Usuario> _userManager;
        public IWebHostEnvironment HostEnvironment { get; }

        public ReviewsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<Usuario> userManager)
        {
            _context = context;
            HostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Reviews
        //[HttpGet("Reviews/page?")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? pageSize, int? page)
        {
            /*var applicationDbContext = _context.Reviews.Include(r => r.Usuario);
            var getawaiter = await applicationDbContext.ToListAsync();*/
            IQueryable<Review> reviewsIQ = from s in _context.Reviews.Include(x => x.Usuario) select s;

            pageSize = (pageSize ?? 5);
            page = (page ?? 1);
            ViewBag.PageSize = pageSize;

            return View(await reviewsIQ.OrderByDescending(x => x.FechaPub).ToPagedListAsync(page.Value, pageSize.Value));
        }

        // GET: Reviews/Create
        [HttpGet]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Create()
        {
            var uid = await _userManager.GetUserAsync(User);
            ViewData["UID"] = uid.Id;
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Create([Bind("ReviewId,Titulo,Descripcion,FechaPub,Img,Contenido,Estrellas,UsuarioId,Foto")] Review review)
        {
            if (ModelState.IsValid)
            {
                if (review.Foto != null)
                {
                    if (!string.IsNullOrEmpty(review.Img))
                    {
                        var filePath = Path.Combine(HostEnvironment.WebRootPath, "img/posts", review.Img);
                        System.IO.File.Delete(filePath);
                    }
                    review.Img = ProcessUploadFile(review);
                }
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles =("administrador"))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var review = await _context.Reviews.FindAsync(id);

            if (review.UsuarioId == user.Id) 
            {
                if (review == null)
                {
                    return NotFound();
                }
                return View(review);
            }
            else
            {
                string url = "/Identity/Account/AccessDenied";
                return Redirect(url);
            }
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,Titulo,Descripcion,FechaPub,Img,Contenido,Estrellas,UsuarioId,Foto")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (review.Foto != null)
                    {
                        if (!string.IsNullOrEmpty(review.Img))
                        {
                            var filePath = Path.Combine(HostEnvironment.WebRootPath, "img/posts", review.Img);
                            System.IO.File.Delete(filePath);
                        }
                        review.Img = ProcessUploadFile(review);
                    }

                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
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
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles =("administrador"))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            var user = await _userManager.GetUserAsync(User);

            if (review == null)
            {
                return NotFound();
            }

            if(review.UsuarioId == user.Id)
            {
                return View(review);
            }
            else
            {
                string url = "/Identity/Account/AccessDenied";
                return Redirect(url);
            }
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =("administrador"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string ProcessUploadFile(Review review)
        {
            if (review.Foto == null)
                return string.Empty;

            var uploadFolder = Path.Combine(HostEnvironment.WebRootPath, "img/posts");
            var fileName = $"{Guid.NewGuid()}_{review.Foto.FileName}";
            var filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                review.Foto.CopyTo(stream);
            }
            return fileName;
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }
    }
}
