using DrustvenaPlatformaVideoIgara.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrustvenaPlatformaVideoIgara.Controllers
{
    public class CartsController : Controller
    {
        private readonly SteamContext _context;

        public CartsController(SteamContext context)
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

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Checkout()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId.Value);

            var paymentMethods = await _context.PaymentMethods.ToListAsync();

            var totalPrice = cart?.CartItems.Sum(ci => ci.Price) ?? 0;

            var viewModel = new CheckoutViewModel
            {
                Cart = cart,
                WalletBalance = wallet.Balance,
                PaymentMethods = paymentMethods,
                TotalPrice = totalPrice // Add TotalPrice to the ViewModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(CheckoutViewModel viewModel)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId.Value);

            if (cart == null || wallet == null)
            {
                return RedirectToAction("Index");
            }

            var totalPrice = cart.CartItems.Sum(ci => ci.Price);

            if (wallet.Balance < totalPrice)
            {
                ModelState.AddModelError("", "Insufficient wallet balance.");
                return RedirectToAction("Checkout");
            }

            // Deduct wallet balance
            wallet.Balance -= totalPrice;
            _context.Update(wallet);

            // Create Invoice
            var invoice = new Invoice
            {
                UserId = userId.Value,
                DateIssued = DateTime.Now,
                PaymentMethodId = viewModel.SelectedPaymentMethod,
                TotalPrice = totalPrice
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Create InvoiceItems
            foreach (var cartItem in cart.CartItems)
            {
                var invoiceItem = new InvoiceItem
                {
                    InvoiceId = invoice.InvoiceId,
                    ProductId = cartItem.ProductId,
                    Price = cartItem.Price
                };
                _context.InvoiceItems.Add(invoiceItem);

                // Remove CartItem
                _context.CartItems.Remove(cartItem);
            }

            // Clear the cart
            cart.TotalPrice = 0;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Invoices", new { id = invoice.InvoiceId });
        }
    }
}
