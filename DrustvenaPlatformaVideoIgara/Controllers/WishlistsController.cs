using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrustvenaPlatformaVideoIgara.Models;
using DrustvenaPlatformaVideoIgara.ViewModels;

namespace DrustvenaPlatformaVideoIgara.Controllers
{
    public class WishlistsController : Controller
    {
        private readonly SteamContext _context;

        public WishlistsController(SteamContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId.Value);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .Where(c => c.UserId == userId.Value)
                .FirstOrDefaultAsync();

            var viewModel = new WishlistViewModel
            {
                Wishlist = wishlist,
                CartItems = cart?.CartItems ?? new List<CartItem>(), // If cart is null, pass an empty list
                UserId = userId.Value // Pass the user ID to the view
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int wishlistItemId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var wishlistItem = await _context.WishlistItems
                .Include(wi => wi.Product)
                .FirstOrDefaultAsync(wi => wi.WishlistItemId == wishlistItemId);

            if (wishlistItem != null)
            {
                // Add product to the cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId.Value);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId.Value };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = wishlistItem.Product.ProductId,
                    Price = wishlistItem.Product.Price
                };

                // Check if item already exists in cart
                if (!_context.CartItems.Any(ci => ci.CartId == cart.CartId && ci.ProductId == wishlistItem.Product.ProductId))
                {
                    _context.CartItems.Add(cartItem);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int wishlistItemId)
        {
            var wishlistItem = await _context.WishlistItems.FindAsync(wishlistItemId);
            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WishlistId == id);
            if (wishlist == null)
            {
                return NotFound();
            }

            return View(wishlist);
        }

        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WishlistId,UserId")] Wishlist wishlist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wishlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", wishlist.UserId);
            return View(wishlist);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", wishlist.UserId);
            return View(wishlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WishlistId,UserId")] Wishlist wishlist)
        {
            if (id != wishlist.WishlistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wishlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishlistExists(wishlist.WishlistId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", wishlist.UserId);
            return View(wishlist);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WishlistId == id);
            if (wishlist == null)
            {
                return NotFound();
            }

            return View(wishlist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WishlistExists(int id)
        {
            return _context.Wishlists.Any(e => e.WishlistId == id);
        }
    }
}
