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
    public class WishlistItemsController : Controller
    {
        private readonly SteamContext _context;

        public WishlistItemsController(SteamContext context)
        {
            _context = context;
        }

        // GET: WishlistItems
        public async Task<IActionResult> Index()
        {
            var steamContext = _context.WishlistItems.Include(w => w.Product).Include(w => w.Wishlist);
            return View(await steamContext.ToListAsync());
        }

        // GET: WishlistItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems
                .Include(w => w.Product)
                .Include(w => w.Wishlist)
                .FirstOrDefaultAsync(m => m.WishlistItemId == id);
            if (wishlistItem == null)
            {
                return NotFound();
            }

            return View(wishlistItem);
        }

        // GET: WishlistItems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            ViewData["WishlistId"] = new SelectList(_context.Wishlists, "WishlistId", "WishlistId");
            return View();
        }

        // POST: WishlistItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WishlistItemId,WishlistId,ProductId")] WishlistItem wishlistItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wishlistItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", wishlistItem.ProductId);
            ViewData["WishlistId"] = new SelectList(_context.Wishlists, "WishlistId", "WishlistId", wishlistItem.WishlistId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems.FindAsync(id);
            if (wishlistItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", wishlistItem.ProductId);
            ViewData["WishlistId"] = new SelectList(_context.Wishlists, "WishlistId", "WishlistId", wishlistItem.WishlistId);
            return View(wishlistItem);
        }

        // POST: WishlistItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WishlistItemId,WishlistId,ProductId")] WishlistItem wishlistItem)
        {
            if (id != wishlistItem.WishlistItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wishlistItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishlistItemExists(wishlistItem.WishlistItemId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", wishlistItem.ProductId);
            ViewData["WishlistId"] = new SelectList(_context.Wishlists, "WishlistId", "WishlistId", wishlistItem.WishlistId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems
                .Include(w => w.Product)
                .Include(w => w.Wishlist)
                .FirstOrDefaultAsync(m => m.WishlistItemId == id);
            if (wishlistItem == null)
            {
                return NotFound();
            }

            return View(wishlistItem);
        }

        // POST: WishlistItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wishlistItem = await _context.WishlistItems.FindAsync(id);
            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WishlistItemExists(int id)
        {
            return _context.WishlistItems.Any(e => e.WishlistItemId == id);
        }
    }
}
