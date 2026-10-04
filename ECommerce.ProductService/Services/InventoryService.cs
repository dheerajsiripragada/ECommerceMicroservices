using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<List<Inventory>> GetAllAsync()
        {
            return await _inventoryRepository.GetAllAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _inventoryRepository.GetByIdAsync(id);
        }

        public async Task<Inventory?> GetByProductIdAsync(int productId)
        {
            return await _inventoryRepository.GetByProductIdAsync(productId);
        }

        public async Task AddAsync(Inventory inventory)
        {
            await _inventoryRepository.AddAsync(inventory);
        }

        public async Task UpdateAsync(Inventory inventory)
        {
            await _inventoryRepository.UpdateAsync(inventory);
        }

        public async Task DeleteAsync(int id)
        {
            await _inventoryRepository.DeleteAsync(id);
        }
        public async Task ReserveAsync(int productId, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than 0.");

            var inventory =
                await _inventoryRepository.GetByProductIdAsync(productId);

            if (inventory == null)
                throw new Exception(
                    $"Inventory not found for product {productId}");

            var availableQuantity =
                inventory.Quantity - inventory.ReservedQuantity;

            if (quantity > availableQuantity)
                throw new Exception("Not enough stock available.");

            inventory.ReservedQuantity += quantity;

            await _inventoryRepository.UpdateAsync(inventory);
        }
        public async Task ConfirmInventoryAsync(int productId, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than 0.");

            var inventory =
                await _inventoryRepository.GetByProductIdAsync(productId);

            if (inventory == null)
                throw new Exception(
                    $"Inventory not found for product {productId}");

            if (inventory.ReservedQuantity < quantity)
                throw new Exception("Reserved stock is insufficient.");

            inventory.Quantity -= quantity;
            inventory.ReservedQuantity -= quantity;

            await _inventoryRepository.UpdateAsync(inventory);
        }

        public async Task ReleaseInventoryAsync(int productId, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than 0.");

            var inventory =
                await _inventoryRepository.GetByProductIdAsync(productId);

            if (inventory == null)
                throw new Exception(
                    $"Inventory not found for product {productId}");

            if (inventory.ReservedQuantity < quantity)
                throw new Exception("Reserved stock is insufficient.");

            inventory.ReservedQuantity -= quantity;

            await _inventoryRepository.UpdateAsync(inventory);
        }
    }
}