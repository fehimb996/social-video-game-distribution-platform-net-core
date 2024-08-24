using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrustvenaPlatformaVideoIgara.Models;

namespace DrustvenaPlatformaVideoIgara.Controllers
{
    public class ProductGenresController : Controller
    {
        private readonly SteamContext _context;

        public ProductGenresController(SteamContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var steamContext = _context.ProductGenres.Include(p => p.Genre).Include(p => p.Product);
            return View(await steamContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productGenre = await _context.ProductGenres
                .Include(p => p.Genre)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductGenreId == id);
            if (productGenre == null)
            {
                return NotFound();
            }

            return View(productGenre);
        }

        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreName");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductGenreId,GenreId,ProductId")] ProductGenre productGenre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productGenre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreName", productGenre.GenreId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productGenre.ProductId);
            return View(productGenre);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productGenre = await _context.ProductGenres.FindAsync(id);
            if (productGenre == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreName", productGenre.GenreId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productGenre.ProductId);
            return View(productGenre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductGenreId,GenreId,ProductId")] ProductGenre productGenre)
        {
            if (id != productGenre.ProductGenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productGenre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductGenreExists(productGenre.ProductGenreId))
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
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreName", productGenre.GenreId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productGenre.ProductId);
            return View(productGenre);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productGenre = await _context.ProductGenres
                .Include(p => p.Genre)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductGenreId == id);
            if (productGenre == null)
            {
                return NotFound();
            }

            return View(productGenre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productGenre = await _context.ProductGenres.FindAsync(id);
            if (productGenre != null)
            {
                _context.ProductGenres.Remove(productGenre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductGenreExists(int id)
        {
            return _context.ProductGenres.Any(e => e.ProductGenreId == id);
        }
    }
}
