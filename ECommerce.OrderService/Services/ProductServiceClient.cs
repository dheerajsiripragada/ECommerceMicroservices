using System.Net.Http.Json;
using ECommerce.OrderService.Interfaces;

namespace ECommerce.OrderService.Services
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductServiceClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ProductInfo?> GetProductAsync(int productId)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ProductInfo>(
                $"api/Product/get-products-by-id/{productId}");
        }
        public async Task ReserveInventoryAsync(int productId,int quantity)
        {
            AddAuthorizationHeader();
            AddAuthorizationHeader();
            var request = new
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Inventory/reserve",
                request);

            response.EnsureSuccessStatusCode();
        }
        public async Task ConfirmInventoryAsync(int productId, int quantity)
        {
            AddAuthorizationHeader();
            var request = new
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Inventory/confirm",
                request);

            response.EnsureSuccessStatusCode();
        }
        public async Task ReleaseInventoryAsync(int productId, int quantity)
        {
            var request = new
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Inventory/release",
                request);

            response.EnsureSuccessStatusCode();
        }
        private void AddAuthorizationHeader()
        {
            var token =
                _httpContextAccessor.HttpContext?
                    .Request.Headers["Authorization"]
                    .FirstOrDefault();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add(
                    "Authorization",
                    token);
            }
        }
    }
}