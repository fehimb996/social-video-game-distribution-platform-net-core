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

        // GET: ProductGenres
        public async Task<IActionResult> Index()
        {
            var steamContext = _context.ProductGenres.Include(p => p.Genre).Include(p => p.Product);
            return View(await steamContext.ToListAsync());
        }

        // GET: ProductGenres/Details/5
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

        // GET: ProductGenres/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: ProductGenres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId", productGenre.GenreId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productGenre.ProductId);
            return View(productGenre);
        }

        // GET: ProductGenres/Edit/5
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
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId", productGenre.GenreId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productGenre.ProductId);
            return View(productGenre);
        }

        // POST: ProductGenres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId", productGenre.GenreId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productGenre.ProductId);
            return View(productGenre);
        }

        // GET: ProductGenres/Delete/5
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

        // POST: ProductGenres/Delete/5
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
