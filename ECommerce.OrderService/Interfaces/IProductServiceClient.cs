namespace ECommerce.OrderService.Interfaces
{
    public interface IProductServiceClient
    {
        Task<ProductInfo?> GetProductAsync(int productId);
        Task ReserveInventoryAsync(int productId,int quantity);
        Task ConfirmInventoryAsync(int productId,int quantity);
        Task ReleaseInventoryAsync(int productId,int quantity);
    }
    public class ProductInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}