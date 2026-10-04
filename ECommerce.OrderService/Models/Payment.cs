namespace ECommerce.OrderService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public Order Order { get; set; }
    }
}