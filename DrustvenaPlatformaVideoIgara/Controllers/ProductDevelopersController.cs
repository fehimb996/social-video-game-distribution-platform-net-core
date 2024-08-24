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
    public class ProductDevelopersController : Controller
    {
        private readonly SteamContext _context;

        public ProductDevelopersController(SteamContext context)
        {
            _context = context;
        }

        // GET: ProductDevelopers
        public async Task<IActionResult> Index()
        {
            var steamContext = _context.ProductDevelopers.Include(p => p.Developer).Include(p => p.Product);
            return View(await steamContext.ToListAsync());
        }

        // GET: ProductDevelopers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDeveloper = await _context.ProductDevelopers
                .Include(p => p.Developer)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductDeveloperId == id);
            if (productDeveloper == null)
            {
                return NotFound();
            }

            return View(productDeveloper);
        }

        // GET: ProductDevelopers/Create
        public IActionResult Create()
        {
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "DeveloperId", "DeveloperName");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: ProductDevelopers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductDeveloperId,DeveloperId,ProductId")] ProductDeveloper productDeveloper)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productDeveloper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "DeveloperId", "DeveloperName", productDeveloper.DeveloperId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productDeveloper.ProductId);
            return View(productDeveloper);
        }

        // GET: ProductDevelopers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDeveloper = await _context.ProductDevelopers.FindAsync(id);
            if (productDeveloper == null)
            {
                return NotFound();
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "DeveloperId", "DeveloperName", productDeveloper.DeveloperId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productDeveloper.ProductId);
            return View(productDeveloper);
        }

        // POST: ProductDevelopers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductDeveloperId,DeveloperId,ProductId")] ProductDeveloper productDeveloper)
        {
            if (id != productDeveloper.ProductDeveloperId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productDeveloper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDeveloperExists(productDeveloper.ProductDeveloperId))
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
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "DeveloperId", "DeveloperName", productDeveloper.DeveloperId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productDeveloper.ProductId);
            return View(productDeveloper);
        }

        // GET: ProductDevelopers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDeveloper = await _context.ProductDevelopers
                .Include(p => p.Developer)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductDeveloperId == id);
            if (productDeveloper == null)
            {
                return NotFound();
            }

            return View(productDeveloper);
        }

        // POST: ProductDevelopers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productDeveloper = await _context.ProductDevelopers.FindAsync(id);
            if (productDeveloper != null)
            {
                _context.ProductDevelopers.Remove(productDeveloper);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDeveloperExists(int id)
        {
            return _context.ProductDevelopers.Any(e => e.ProductDeveloperId == id);
        }
    }
}
