using ECommerce.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithOne(p => p.Inventory)
                .HasForeignKey<Inventory>(i => i.ProductId);
            var categories = new List<Category>
{
    new Category
    {
        Id = 1,
        Name = "Electronics"
    },
    new Category
    {
        Id = 2,
        Name = "Books"
    },
    new Category
    {
        Id = 3,
        Name = "Clothing"
    },
    new Category
    {
        Id = 4,
        Name = "Home & Kitchen"
    }
};

            var products = new List<Product>
{
    new Product
    {
        Id = 1,
        Name = "Laptop",
        Description = "15-inch laptop with 16GB RAM and 512GB SSD",
        Price = 75000,
        ImageUrl = "https://placehold.co/600x400?text=Laptop",
        CategoryId = 1
    },
    new Product
    {
        Id = 2,
        Name = "Smartphone",
        Description = "Android smartphone with 128GB storage",
        Price = 45000,
        ImageUrl = "https://placehold.co/600x400?text=Smartphone",
        CategoryId = 1
    },
    new Product
    {
        Id = 3,
        Name = "Wireless Headphones",
        Description = "Noise-cancelling wireless headphones",
        Price = 5000,
        ImageUrl = "https://placehold.co/600x400?text=Headphones",
        CategoryId = 1
    },
    new Product
    {
        Id = 4,
        Name = "Clean Code",
        Description = "A book about writing clean and maintainable software",
        Price = 1200,
        ImageUrl = "https://placehold.co/600x400?text=Clean+Code",
        CategoryId = 2
    },
    new Product
    {
        Id = 5,
        Name = "The Pragmatic Programmer",
        Description = "A guide to becoming a better software developer",
        Price = 1500,
        ImageUrl = "https://placehold.co/600x400?text=Pragmatic+Programmer",
        CategoryId = 2
    },
    new Product
    {
        Id = 6,
        Name = "Cotton T-Shirt",
        Description = "Comfortable round-neck cotton T-shirt",
        Price = 800,
        ImageUrl = "https://placehold.co/600x400?text=Cotton+T-Shirt",
        CategoryId = 3
    },
    new Product
    {
        Id = 7,
        Name = "Running Shoes",
        Description = "Lightweight shoes suitable for running and training",
        Price = 2500,
        ImageUrl = "https://placehold.co/600x400?text=Running+Shoes",
        CategoryId = 3
    },
    new Product
    {
        Id = 8,
        Name = "Coffee Maker",
        Description = "Automatic coffee maker for home use",
        Price = 3500,
        ImageUrl = "https://placehold.co/600x400?text=Coffee+Maker",
        CategoryId = 4
    }
};

            var inventories = new List<Inventory>
{
    new Inventory
    {
        Id = 1,
        ProductId = 1,
        Quantity = 10
    },
    new Inventory
    {
        Id = 2,
        ProductId = 2,
        Quantity = 25
    },
    new Inventory
    {
        Id = 3,
        ProductId = 3,
        Quantity = 30
    },
    new Inventory
    {
        Id = 4,
        ProductId = 4,
        Quantity = 15
    },
    new Inventory
    {
        Id = 5,
        ProductId = 5,
        Quantity = 20
    },
    new Inventory
    {
        Id = 6,
        ProductId = 6,
        Quantity = 50
    },
    new Inventory
    {
        Id = 7,
        ProductId = 7,
        Quantity = 35
    },
    new Inventory
    {
        Id = 8,
        ProductId = 8,
        Quantity = 12
    }
};

            modelBuilder.Entity<Category>().HasData(categories);
            modelBuilder.Entity<Product>().HasData(products);
            modelBuilder.Entity<Inventory>().HasData(inventories);
        }
    }
}