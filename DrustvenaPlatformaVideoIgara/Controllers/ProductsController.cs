using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrustvenaPlatformaVideoIgara.Models;
using Microsoft.AspNetCore.Hosting;

namespace DrustvenaPlatformaVideoIgara.Controllers
{
    public class ProductsController : Controller
    {
        private readonly SteamContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(SteamContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch 10 random products
            var randomProducts = await _context.Products
                .OrderBy(r => Guid.NewGuid())
                .Take(10)
                .ToListAsync();

            // Fetch top-selling products by unique users who bought the product
            var topSellingProducts = await _context.InvoiceItems
                .Join(_context.Invoices,
                      ii => ii.InvoiceId,
                      i => i.InvoiceId,
                      (ii, i) => new { ii.Product, i.UserId })
                .GroupBy(x => new { x.Product.ProductId, x.Product.ProductName, x.Product.Price })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Price = g.Key.Price,
                    TimesSold = g.Select(x => x.UserId).Distinct().Count() // Count of unique users who bought the product
                })
                .OrderByDescending(g => g.TimesSold) // Order by the number of unique users
                .Take(10)
                .ToListAsync();

            // Fetch products under 10 bucks but greater than 5 that have never been sold
            var productsUnder10Bucks = await _context.Products
                .Where(p => p.Price > 5.00m && p.Price < 10.00m &&
                            !_context.InvoiceItems.Any(ii => ii.ProductId == p.ProductId))
                .OrderBy(p => p.Price)
                .ToListAsync();

            // Fetch products under 5 bucks but greater than 0.01 that have never been sold
            var productsUnder5Bucks = await _context.Products
                .Where(p => p.Price > 0.01m && p.Price < 5.00m &&
                            !_context.InvoiceItems.Any(ii => ii.ProductId == p.ProductId))
                .OrderBy(p => p.Price)
                .ToListAsync();

            // Fetch free products that have never been sold
            var freeProducts = await _context.Products
                .Where(p => p.Price == 0.00m &&
                            !_context.InvoiceItems.Any(ii => ii.ProductId == p.ProductId))
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            // Prepare the view model
            var viewModel = new ProductsViewModel
            {
                RandomProducts = randomProducts,
                TopSellingProducts = topSellingProducts.Select(p => new TopSellingProduct
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price
                }),
                ProductsUnder10Bucks = productsUnder10Bucks,
                ProductsUnder5Bucks = productsUnder5Bucks,
                FreeProducts = freeProducts
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
                .Include(p => p.ProductGenres)
                .ThenInclude(pg => pg.Genre)
                .Include(p => p.ProductPlatforms)
                .ThenInclude(pp => pp.Platform)
                .Include(p => p.ProductPublishers)
                .ThenInclude(pp => pp.Publisher)
                .Include(p => p.ProductDevelopers)
                .ThenInclude(pd => pd.Developer)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            bool isInCart = false;
            bool isOwned = false;
            bool isInWishlist = false;
            int? userId = HttpContext.Session.GetInt32("UserId");
            Review userReview = null;
            if (userId.HasValue)
            {
                isInCart = await _context.CartItems
                    .AnyAsync(ci => ci.Cart.UserId == userId.Value && ci.ProductId == id.Value);

                isOwned = await _context.InvoiceItems
                    .AnyAsync(ii => ii.Invoice.UserId == userId.Value && ii.ProductId == id.Value);

                isInWishlist = await _context.WishlistItems
                    .AnyAsync(wi => wi.Wishlist.UserId == userId.Value && wi.ProductId == id.Value);

                userReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.ProductId == id.Value);
            }

            var reviews = await _context.Reviews
                .Include(r => r.User) // Include the User navigation property
                .Where(r => r.ProductId == id.Value)
                .ToListAsync();

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                IsInCart = isInCart,
                IsUserLoggedIn = userId.HasValue,
                IsOwned = isOwned,
                IsInWishlist = isInWishlist,
                Reviews = reviews,
                UserReview = userReview,
                UserId = userId,
                Genres = product.ProductGenres.Select(pg => pg.Genre).ToList(),
                Platforms = product.ProductPlatforms.Select(pp => pp.Platform).ToList(),
                Publishers = product.ProductPublishers.Select(pp => pp.Publisher).ToList(),
                Developers = product.ProductDevelopers.Select(pd => pd.Developer).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = new CartItem { CartId = cart.CartId, ProductId = productId, Price = await _context.Products.Where(p => p.ProductId == productId).Select(p => p.Price).FirstOrDefaultAsync() };
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId.Value);

            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId.Value };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            var wishlistItem = new WishlistItem { WishlistId = wishlist.WishlistId, ProductId = productId };
            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, string comment, bool rating)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.ProductId == productId);

            if (existingReview == null)
            {
                var review = new Review
                {
                    UserId = userId.Value,
                    ProductId = productId,
                    Comment = comment,
                    Rating = rating
                };
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> EditReview(int reviewId, string comment, bool rating)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            review.Comment = comment;
            review.Rating = rating;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = review.ProductId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = review.ProductId });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        product.ImagePath = await SaveProfilePicture(imageFile);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Image upload failed: {ex.Message}");
                        return View(product); // Return view with model to retain input
                    }
                }
                else
                {
                    // Assign a default image path if no image is uploaded
                    product.ImagePath = "images/default.png";
                }

                try
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Product creation failed: {ex.Message}");
                }
            }
            else
            {
                // Log the validation errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Property: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            return View(product); // Return view with model to retain input
        }

        private async Task<string> SaveProfilePicture(IFormFile profilePicture)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "ProductCover");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePicture.FileName;
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile picture: {ex.Message}");
                throw;
            }

            return "/images/ProductCover/" + uniqueFileName;
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
        public async Task<IActionResult> Edit(int id, Product product, IFormFile imageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Delete the old image file
                        if (!string.IsNullOrEmpty(product.ImagePath))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save the new image file
                        product.ImagePath = await SaveProfilePicture(imageFile);
                    }

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
