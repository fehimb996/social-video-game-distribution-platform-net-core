namespace DrustvenaPlatformaVideoIgara.Models
{
    public class CheckoutViewModel
    {
        public Cart Cart { get; set; } = null!;
        public decimal WalletBalance { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
        public int SelectedPaymentMethod { get; set; } 
        public decimal TotalPrice { get; set; }
    }
}
