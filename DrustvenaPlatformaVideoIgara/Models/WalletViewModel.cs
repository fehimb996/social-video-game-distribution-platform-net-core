namespace DrustvenaPlatformaVideoIgara.Models
{
    public class WalletViewModel
    {
        public decimal Balance { get; set; }
        public IEnumerable<PaymentMethod> PaymentMethods { get; set; }
    }
}
