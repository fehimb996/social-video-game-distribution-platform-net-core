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
    public class ProductPublishersController : Controller
    {
        private readonly SteamContext _context;

        public ProductPublishersController(SteamContext context)
        {
            _context = context;
        }

        // GET: ProductPublishers
        public async Task<IActionResult> Index()
        {
            var steamContext = _context.ProductPublishers.Include(p => p.Product).Include(p => p.Publisher);
            return View(await steamContext.ToListAsync());
        }

        // GET: ProductPublishers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPublisher = await _context.ProductPublishers
                .Include(p => p.Product)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(m => m.ProductPublisherId == id);
            if (productPublisher == null)
            {
                return NotFound();
            }

            return View(productPublisher);
        }

        // GET: ProductPublishers/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId");
            return View();
        }

        // POST: ProductPublishers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductPublisherId,PublisherId,ProductId")] ProductPublisher productPublisher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productPublisher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productPublisher.ProductId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", productPublisher.PublisherId);
            return View(productPublisher);
        }

        // GET: ProductPublishers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPublisher = await _context.ProductPublishers.FindAsync(id);
            if (productPublisher == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productPublisher.ProductId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", productPublisher.PublisherId);
            return View(productPublisher);
        }

        // POST: ProductPublishers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductPublisherId,PublisherId,ProductId")] ProductPublisher productPublisher)
        {
            if (id != productPublisher.ProductPublisherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productPublisher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductPublisherExists(productPublisher.ProductPublisherId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productPublisher.ProductId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherId", productPublisher.PublisherId);
            return View(productPublisher);
        }

        // GET: ProductPublishers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPublisher = await _context.ProductPublishers
                .Include(p => p.Product)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(m => m.ProductPublisherId == id);
            if (productPublisher == null)
            {
                return NotFound();
            }

            return View(productPublisher);
        }

        // POST: ProductPublishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productPublisher = await _context.ProductPublishers.FindAsync(id);
            if (productPublisher != null)
            {
                _context.ProductPublishers.Remove(productPublisher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductPublisherExists(int id)
        {
            return _context.ProductPublishers.Any(e => e.ProductPublisherId == id);
        }
    }
}
