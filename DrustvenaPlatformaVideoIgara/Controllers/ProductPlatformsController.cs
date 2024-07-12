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
    public class ProductPlatformsController : Controller
    {
        private readonly SteamContext _context;

        public ProductPlatformsController(SteamContext context)
        {
            _context = context;
        }

        // GET: ProductPlatforms
        public async Task<IActionResult> Index()
        {
            var steamContext = _context.ProductPlatforms.Include(p => p.Platform).Include(p => p.Product);
            return View(await steamContext.ToListAsync());
        }

        // GET: ProductPlatforms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPlatform = await _context.ProductPlatforms
                .Include(p => p.Platform)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductPlatformId == id);
            if (productPlatform == null)
            {
                return NotFound();
            }

            return View(productPlatform);
        }

        // GET: ProductPlatforms/Create
        public IActionResult Create()
        {
            ViewData["PlatformId"] = new SelectList(_context.Platforms, "PlatformId", "PlatformId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: ProductPlatforms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductPlatformId,PlatformId,ProductId")] ProductPlatform productPlatform)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productPlatform);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlatformId"] = new SelectList(_context.Platforms, "PlatformId", "PlatformId", productPlatform.PlatformId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productPlatform.ProductId);
            return View(productPlatform);
        }

        // GET: ProductPlatforms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPlatform = await _context.ProductPlatforms.FindAsync(id);
            if (productPlatform == null)
            {
                return NotFound();
            }
            ViewData["PlatformId"] = new SelectList(_context.Platforms, "PlatformId", "PlatformId", productPlatform.PlatformId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productPlatform.ProductId);
            return View(productPlatform);
        }

        // POST: ProductPlatforms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductPlatformId,PlatformId,ProductId")] ProductPlatform productPlatform)
        {
            if (id != productPlatform.ProductPlatformId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productPlatform);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductPlatformExists(productPlatform.ProductPlatformId))
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
            ViewData["PlatformId"] = new SelectList(_context.Platforms, "PlatformId", "PlatformId", productPlatform.PlatformId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productPlatform.ProductId);
            return View(productPlatform);
        }

        // GET: ProductPlatforms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPlatform = await _context.ProductPlatforms
                .Include(p => p.Platform)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductPlatformId == id);
            if (productPlatform == null)
            {
                return NotFound();
            }

            return View(productPlatform);
        }

        // POST: ProductPlatforms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productPlatform = await _context.ProductPlatforms.FindAsync(id);
            if (productPlatform != null)
            {
                _context.ProductPlatforms.Remove(productPlatform);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductPlatformExists(int id)
        {
            return _context.ProductPlatforms.Any(e => e.ProductPlatformId == id);
        }
    }
}
