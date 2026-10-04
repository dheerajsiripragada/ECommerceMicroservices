using ClosedXML.Excel;
using ECommerce.ProductService.Interfaces;
using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task ImportProductsAsync(Stream fileStream)
        {
            const int batchSize = 500;

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);

            var headerRow = worksheet.FirstRowUsed();

            var headers = headerRow.CellsUsed()
                .ToDictionary(
                    cell => cell.GetString().Trim(),
                    cell => cell.Address.ColumnNumber,
                    StringComparer.OrdinalIgnoreCase);

            var productIds = new HashSet<int>();
            var productNames = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            var requiredHeaders = new[]
            {
        "Id",
        "Name",
        "Description",
        "Price",
        "CategoryId",
        "Quantity",
        "ImageUrl"
    };

            var missingHeaders = requiredHeaders
                .Where(header => !headers.ContainsKey(header))
                .ToList();

            if (missingHeaders.Any())
            {
                throw new InvalidOperationException(
                    $"Missing required columns: {string.Join(", ", missingHeaders)}");
            }

            var rows = new List<IXLRow>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (!row.IsEmpty())
                {
                    rows.Add(row);
                }

                if (rows.Count == batchSize)
                {
                    await ProcessBatchAsync(
                        rows,
                        headers,
                        productIds,
                        productNames);

                    rows.Clear();
                }
            }

            if (rows.Any())
            {
                await ProcessBatchAsync(
                    rows,
                    headers,
                    productIds,
                    productNames);
            }
        }
        private async Task ProcessBatchAsync(
    List<IXLRow> rows,
    Dictionary<string, int> headers,
    HashSet<int> productIds,
    HashSet<string> productNames)
        {
            var productIdList = new List<int>();
            var categoryIdList = new List<int>();

            foreach (var row in rows)
            {
                var idValue = row.Cell(headers["Id"]).GetString().Trim();
                var categoryIdValue = row.Cell(headers["CategoryId"]).GetString().Trim();

                if (!string.IsNullOrWhiteSpace(idValue) &&
                    int.TryParse(idValue, out var id) &&
                    id > 0)
                {
                    productIdList.Add(id);
                }

                if (int.TryParse(categoryIdValue, out var categoryId))
                {
                    categoryIdList.Add(categoryId);
                }
            }

            productIdList = productIdList.Distinct().ToList();
            categoryIdList = categoryIdList.Distinct().ToList();

            var existingProducts =
                await _productRepository.GetByIdsAsync(productIdList);

            var existingCategories =
                await _categoryRepository.GetByIdsAsync(categoryIdList);

            var productDictionary = existingProducts
                .ToDictionary(p => p.Id);

            var categoryDictionary = existingCategories
                .ToDictionary(c => c.Id);

            var productsToAdd = new List<Product>();
            var productsToUpdate = new List<Product>();

            foreach (var row in rows)
            {
                var rowNumber = row.RowNumber();

                var idValue = row.Cell(headers["Id"]).GetString().Trim();
                var name = row.Cell(headers["Name"]).GetString().Trim();
                var description =
                    row.Cell(headers["Description"]).GetString().Trim();
                var priceValue =
                    row.Cell(headers["Price"]).GetString().Trim();
                var categoryIdValue =
                    row.Cell(headers["CategoryId"]).GetString().Trim();
                var quantityValue =
                    row.Cell(headers["Quantity"]).GetString().Trim();
                var imageUrl =
                    row.Cell(headers["ImageUrl"]).GetString().Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: Name is required.");
                }

                if (!decimal.TryParse(priceValue, out var price) || price < 0)
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: Price must be a valid non-negative number.");
                }

                if (!int.TryParse(categoryIdValue, out var categoryId))
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: CategoryId must be a valid integer.");
                }

                if (!categoryDictionary.ContainsKey(categoryId))
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: CategoryId {categoryId} does not exist.");
                }

                if (!int.TryParse(quantityValue, out var quantity) || quantity < 0)
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: Quantity must be a valid non-negative integer.");
                }

                int? id = null;

                if (!string.IsNullOrWhiteSpace(idValue))
                {
                    if (!int.TryParse(idValue, out var parsedId) || parsedId <= 0)
                    {
                        throw new InvalidOperationException(
                            $"Row {rowNumber}: Id must be a valid positive integer.");
                    }

                    id = parsedId;
                }

                if (id.HasValue && !productIds.Add(id.Value))
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: Duplicate product Id {id.Value} found in the Excel file.");
                }

                if (!id.HasValue && !productNames.Add(name))
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: Duplicate product name '{name}' found in the Excel file.");
                }

                if (id.HasValue &&
                    productDictionary.TryGetValue(id.Value, out var product))
                {
                    product.Name = name;
                    product.Description = description;
                    product.Price = price;
                    product.CategoryId = categoryId;
                    product.ImageUrl = imageUrl;

                    if (product.Inventory != null)
                    {
                        product.Inventory.Quantity = quantity;
                    }

                    productsToUpdate.Add(product);
                }
                else
                {
                    product = new Product
                    {
                        Name = name,
                        Description = description,
                        Price = price,
                        ImageUrl = imageUrl,
                        CategoryId = categoryId,
                        Inventory = new Inventory
                        {
                            Quantity = quantity
                        }
                    };

                    productsToAdd.Add(product);
                }
            }

            if (productsToAdd.Any() || productsToUpdate.Any())
            {
                await _productRepository.SaveBatchAsync(
                    productsToAdd,
                    productsToUpdate);
            }
        }
    }
}