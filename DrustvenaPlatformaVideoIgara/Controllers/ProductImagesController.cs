using DrustvenaPlatformaVideoIgara.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DrustvenaPlatformaVideoIgara.Controllers
{
    public class ProductImagesController : Controller
    {
        private readonly SteamContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductImagesController(SteamContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProductImages
        public async Task<IActionResult> Index()
        {
            var images = await _context.ProductImages.Include(p => p.Product).ToListAsync();
            return View(images);
        }

        // GET: ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: ProductImages/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: ProductImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageId,ProductId")] ProductImage productImage, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                productImage.ImagePath = await SaveProductImage(imageFile);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productImage.ProductId);
                return View(productImage);
            }

            _context.Add(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productImage.ProductId);
            return View(productImage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImageId,ProductId,ImagePath")] ProductImage productImage, IFormFile imageFile)
        {
            if (id != productImage.ImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        productImage.ImagePath = await SaveProductImage(imageFile);
                    }
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.ImageId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productImage.ProductId);
            return View(productImage);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.ImageId == id);
        }

        private async Task<string> SaveProductImage(IFormFile imagePath)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "ProductImages");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Use the original file name
            var fileName = imagePath.FileName;
            var filePath = Path.Combine(uploadPath, fileName);

            // Check if the file already exists
            if (!System.IO.File.Exists(filePath))
            {
                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagePath.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving profile picture: {ex.Message}");
                    throw;
                }
            }

            return "/images/ProductImages/" + fileName;
        }
    }
}
