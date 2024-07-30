namespace DrustvenaPlatformaVideoIgara.Models
{
    public class CheckoutViewModel
    {
        public Cart Cart { get; set; }
        public decimal WalletBalance { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public int SelectedPaymentMethod { get; set; }
    }
}
