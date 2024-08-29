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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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

            // If id is null, use the logged-in user's ID
            if (id == null)
            {
                if (loggedInUserId == null)
                {
                    return RedirectToAction("Login", "Users");
                }
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

            ViewData["IsFriend"] = loggedInUserId.HasValue ? await IsFriend(loggedInUserId.Value, id.Value) : false;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfriend(int friendId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var friendship = await _context.Friends.FirstOrDefaultAsync(f =>
                (f.UserId1 == loggedInUserId && f.UserId2 == friendId) ||
                (f.UserId1 == friendId && f.UserId2 == loggedInUserId));

            if (friendship != null)
            {
                _context.Friends.Remove(friendship);
                await _context.SaveChangesAsync();
            }

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
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered. Please use a different one.");
            }

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
            // Check if both fields are filled out
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Please enter both email and password.");
                return View(new User { Email = email }); // Pass the email back to the view
            }

            // Find the user by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            // Check if the user exists
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "The email address does not exist.");
                return View(new User { Email = email }); // Pass the email back to the view
            }

            // Check if the password is correct
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return View(new User { Email = email }); // Pass the email back to the view
            }

            // If both the email and password are correct, log the user in
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("NickName", user.NickName);

            // Set authentication cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("NickName", user.NickName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Products"); // Redirect to home page after login
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear the session
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait(); // Sign out from authentication
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
        public async Task<IActionResult> Edit(int id, [Bind("UserId,NickName,FirstName,LastName,Email,Password,ProfileDescription,CountryId,ProfilePicture")] User user, IFormFile ProfilePicture, string ExistingProfilePicture)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            // Remove any existing validation errors for ProfilePicture
            ModelState.Remove(nameof(user.ProfilePicture));

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new profile picture is uploaded, save it
                    if (ProfilePicture != null)
                    {
                        user.ProfilePicture = await SaveProfilePicture(ProfilePicture);
                    }
                    else
                    {
                        // If no new profile picture is uploaded, use the existing one
                        user.ProfilePicture = ExistingProfilePicture;
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

            // Repopulate the ViewData in case of an error
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryName", user.CountryId);
            return View(user);
        }

        private async Task<string> SaveProfilePicture(IFormFile profilePicture)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "ProfilePictures");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = profilePicture.FileName;
            var filePath = Path.Combine(uploadPath, fileName);

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

            return "/images/ProfilePictures/" + fileName;
        }

        public async Task<IActionResult> Wallet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return NotFound();
            }

            var paymentMethods = await _context.PaymentMethods
                .Where(pm => pm.PaymentMethodId != 1) // Exclude Wallet as a payment method
                .ToListAsync();

            var model = new WalletViewModel
            {
                Balance = wallet.Balance,
                PaymentMethods = paymentMethods
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFunds(decimal amount, int paymentMethod)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return NotFound();
            }

            // Check if the payment method is valid
            var paymentMethodExists = await _context.PaymentMethods
                .AnyAsync(pm => pm.PaymentMethodId == paymentMethod && pm.PaymentMethodId != 1);

            if (!paymentMethodExists)
            {
                ModelState.AddModelError(string.Empty, "Invalid payment method.");
                return RedirectToAction("Wallet");
            }

            // Add funds to the wallet
            wallet.Balance += amount;
            _context.Update(wallet);

            // Record the transaction
            var transaction = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                Amount = amount,
                TransactionType = "Credit",
                TransactionDate = DateTime.Now
            };
            _context.WalletTransactions.Add(transaction);

            await _context.SaveChangesAsync();

            return RedirectToAction("Wallet");
        }

        public async Task<IActionResult> Chat()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            var nickName = HttpContext.Session.GetString("NickName");

            if (loggedInUserId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var user = await _context.Users
                .Include(u => u.FriendUserId1Navigations)
                .ThenInclude(f => f.UserId2Navigation)
                .Include(u => u.FriendUserId2Navigations)
                .ThenInclude(f => f.UserId1Navigation)
                .FirstOrDefaultAsync(u => u.UserId == loggedInUserId);

            ViewBag.NickName = nickName;

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetChatHistory(int recipientUserId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return Unauthorized();
            }

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"); // CET/Belgrade time zone

            var messages = await _context.Messages
                .Where(m => (m.UserId1 == loggedInUserId && m.UserId2 == recipientUserId) ||
                            (m.UserId1 == recipientUserId && m.UserId2 == loggedInUserId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    m.MessageContent,
                    SenderNickName = m.UserId1 == loggedInUserId ? _context.Users.Where(u => u.UserId == m.UserId2).Select(u => u.NickName).FirstOrDefault()
                                                                 : _context.Users.Where(u => u.UserId == m.UserId1).Select(u => u.NickName).FirstOrDefault(),
                    Timestamp = TimeZoneInfo.ConvertTimeFromUtc(m.Timestamp, timeZoneInfo).ToString("dd MMM yyyy, HH:mm"), // Format timestamp
                    ProfilePicture = m.UserId1 == loggedInUserId ? _context.Users.Where(u => u.UserId == m.UserId2).Select(u => u.ProfilePicture).FirstOrDefault()
                                                                 : _context.Users.Where(u => u.UserId == m.UserId1).Select(u => u.ProfilePicture).FirstOrDefault()
                })
                .ToListAsync();

            return Json(messages);
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
