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
    public class UserController : Controller
    {
        private readonly SteamContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(SteamContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var steamContext = _context.Users.Include(u => u.Country);
            return View(await steamContext.ToListAsync());
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var user = _context.Users.Find(userId);
            return View(user);
        }

        // GET: Users/Register
        public IActionResult Register()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryName");
            return View();
        }

        // POST: Users/Register
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

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
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

        // GET: Users/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToAction("Login");
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
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

        //// GET: User/Create
        //public IActionResult Create()
        //{
        //    ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId");
        //    return View();
        //}

        //// POST: User/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UserId,NickName,FirstName,LastName,Email,Password,ProfilePicture,ProfileDescription,CountryId")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(user);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId", user.CountryId);
        //    return View(user);
        //}

        // GET: User/Edit/5
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

        // POST: User/Edit/5
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
                    if (ProfilePicture != null)
                    {
                        user.ProfilePicture = await SaveProfilePicture(ProfilePicture);
                    }

                    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
                    if (existingUser != null)
                    {
                        user.Password = existingUser.Password;
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

        // GET: User/Delete/5
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

        // POST: User/Delete/5
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
    }
}
