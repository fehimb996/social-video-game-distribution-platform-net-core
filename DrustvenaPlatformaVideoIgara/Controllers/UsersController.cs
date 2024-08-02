using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrustvenaPlatformaVideoIgara.Models;
using BCrypt.Net;

namespace DrustvenaPlatformaVideoIgara.Controllers
{
    public class UsersController : Controller
    {
        private readonly SteamContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(SteamContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var steamContext = _context.Users.Include(u => u.Country);
            return View(await steamContext.ToListAsync());
        }

        public async Task<IActionResult> Profile(int? id)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                id = loggedInUserId;
            }

            var user = await _context.Users
                .Include(u => u.Country)
                .Include(u => u.FriendUserId1Navigations)
                .Include(u => u.FriendUserId2Navigations)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var gamesCount = await _context.Invoices
                .Where(i => i.UserId == id)
                .SelectMany(i => i.InvoiceItems)
                .Select(id => id.Product)
                .Distinct()
                .CountAsync();

            var reviewsCount = await _context.Reviews
                .Where(r => r.UserId == id)
                .CountAsync();

            var friendsCount = user.FriendUserId1Navigations.Count + user.FriendUserId2Navigations.Count;

            ViewData["GamesCount"] = gamesCount;
            ViewData["ReviewsCount"] = reviewsCount;
            ViewData["FriendsCount"] = friendsCount;
            ViewData["IsOwnProfile"] = id == loggedInUserId;

            // Fetch wallet balance for the user
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == id);

            ViewData["WalletBalance"] = wallet?.Balance ?? 0;

            ViewData["IsFriend"] = await IsFriend(loggedInUserId.Value, id.Value);

            return View(user);
        }


        public async Task<IActionResult> Friends(int id)
        {
            var user = await _context.Users
                .Include(u => u.FriendUserId1Navigations)
                .ThenInclude(f => f.UserId2Navigation)
                .Include(u => u.FriendUserId2Navigations)
                .ThenInclude(f => f.UserId1Navigation)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var friends = user.FriendUserId1Navigations
                .Select(f => f.UserId2Navigation)
                .Union(user.FriendUserId2Navigations.Select(f => f.UserId1Navigation))
                .ToList();

            return View(friends);
        }

        private async Task<bool> IsFriend(int loggedInUserId, int viewedUserId)
        {
            return await _context.Friends.AnyAsync(f =>
                (f.UserId1 == loggedInUserId && f.UserId2 == viewedUserId) ||
                (f.UserId1 == viewedUserId && f.UserId2 == loggedInUserId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFriend(int friendId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var friend = new Friend
            {
                UserId1 = loggedInUserId.Value,
                UserId2 = friendId
            };

            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", new { id = friendId });
        }

        public async Task<IActionResult> Games(int id)
        {
            var user = await _context.Users
        .Include(u => u.Invoices)
        .ThenInclude(i => i.InvoiceItems)
        .ThenInclude(id => id.Product)
        .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var products = user.Invoices
                .SelectMany(i => i.InvoiceItems)
                .Select(id => id.Product)
                .Distinct()
                .ToList();

            return View(products);
        }

        public async Task<IActionResult> Reviews(int id)
        {
            var reviews = await _context.Reviews
               .Include(r => r.Product)
               .Where(r => r.UserId == id)
               .ToListAsync();

            return View(reviews);
        }

        public async Task<IActionResult> Wishlist(int id)
        {
            var user = await _context.Users
                .Include(u => u.Wishlist)
                .ThenInclude(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            // Retrieve the wishlist items directly from the WishlistItems collection
            var wishlistItems = user.Wishlist?.WishlistItems
                .Select(wi => wi.Product)
                .ToList() ?? new List<Product>();

            return View(wishlistItems);
        }

        public IActionResult Register()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserId,NickName,FirstName,LastName,Email,Password,ProfileDescription,CountryId")] User user, IFormFile profilePicture)
        {
            if (ModelState.IsValid)
            {
                // Hash the password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                if (profilePicture != null)
                {
                    user.ProfilePicture = await SaveProfilePicture(profilePicture);
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryName", user.CountryId);
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                // Add user info to session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("NickName", user.NickName);
                return RedirectToAction("Index", "Home"); // Redirect to home page after login
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryName", user.CountryId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,NickName,FirstName,LastName,Email,Password,ProfileDescription,CountryId")] User user, IFormFile ProfilePicture)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a new profile picture is provided
                    if (ProfilePicture != null)
                    {
                        user.ProfilePicture = await SaveProfilePicture(ProfilePicture);
                    }
                    else
                    {
                        // Keep the existing profile picture value
                        var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
                        if (existingUser != null)
                        {
                            user.ProfilePicture = existingUser.ProfilePicture;
                        }
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryName", user.CountryId);
            return View(user);
        }

        private async Task<string> SaveProfilePicture(IFormFile profilePicture)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
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

            return "/images/" + uniqueFileName;
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Country)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public async Task<IActionResult> Library()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Fetch the invoice details for the logged-in user
            var userInvoices = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .ThenInclude(id => id.Product)
                .Where(i => i.UserId == userId)
                .ToListAsync();

            // Extract the products from the invoice details
            var products = userInvoices
                .SelectMany(i => i.InvoiceItems)
                .Select(id => id.Product)
                .ToList();

            return View(products);
        }
    }
}
