using DrustvenaPlatformaVideoIgara.Models;

namespace DrustvenaPlatformaVideoIgara.ViewModels
{
    public class WalletViewModel
    {
        public decimal Balance { get; set; }
        public IEnumerable<PaymentMethod> PaymentMethods { get; set; }
    }
}
