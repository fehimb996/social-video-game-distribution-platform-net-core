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
    public class ProductsController : Controller
    {
        private readonly SteamContext _context;

        public ProductsController(SteamContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch 10 random products
            var randomProducts = await _context.Products
                .OrderBy(r => Guid.NewGuid())
                .Take(10)
                .ToListAsync();

            // Fetch top-selling products
            var topSellingProducts = await _context.InvoiceItems
                .GroupBy(ii => new { ii.Product.ProductId, ii.Product.ProductName, ii.Product.Price })
                .Select(g => new TopSellingProduct
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Price = g.Key.Price
                })
                .OrderByDescending(g => g.Price)
                .Take(10)
                .ToListAsync();

            var viewModel = new ProductsViewModel
            {
                RandomProducts = randomProducts,
                TopSellingProducts = topSellingProducts
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            bool isInCart = false;
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                isInCart = await _context.CartItems
                    .AnyAsync(ci => ci.Cart.UserId == userId.Value && ci.ProductId == id.Value);
            }

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                IsInCart = isInCart,
                IsUserLoggedIn = userId.HasValue
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Users");
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId.Value,
                    TotalPrice = 0
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = productId,
                Price = (await _context.Products.FindAsync(productId)).Price
            };

            _context.CartItems.Add(cartItem);
            cart.TotalPrice += cartItem.Price;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = productId });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDetails,ReleaseDate,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDetails,ReleaseDate,Price")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
