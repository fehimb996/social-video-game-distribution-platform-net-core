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
    public class InvoicesController : Controller
    {
        private readonly SteamContext _context;

        public InvoicesController(SteamContext context)
        {
            _context = context;
        }

        private int? GetLoggedInUserId()
        {
            // Assume you are using session to store user ID
            return HttpContext.Session.GetInt32("UserId");
        }

        // Display a list of invoices for the logged-in user
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetLoggedInUserId();
            if (loggedInUserId == null)
            {
                return Unauthorized();
            }

            var invoices = _context.Invoices
                .Where(i => i.UserId == loggedInUserId)
                .Include(i => i.PaymentMethod)
                .Include(i => i.User);

            return View(await invoices.ToListAsync());
        }

        // Display details for a specific invoice if it belongs to the logged-in user
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loggedInUserId = GetLoggedInUserId();
            if (loggedInUserId == null)
            {
                return Unauthorized();
            }

            var invoice = await _context.Invoices
                .Include(i => i.PaymentMethod)
                .Include(i => i.User)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                .FirstOrDefaultAsync(m => m.InvoiceId == id && m.UserId == loggedInUserId);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        public IActionResult Create()
        {
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvoiceId,UserId,DateIssued,PaymentMethodId,TotalPrice")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", invoice.PaymentMethodId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", invoice.UserId);
            return View(invoice);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", invoice.PaymentMethodId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", invoice.UserId);
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceId,UserId,DateIssued,PaymentMethodId,TotalPrice")] Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceId))
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
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", invoice.PaymentMethodId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", invoice.UserId);
            return View(invoice);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.PaymentMethod)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }
    }
}
