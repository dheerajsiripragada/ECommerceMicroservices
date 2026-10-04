namespace ECommerce.OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }
    }
}