using ECommerce.UserService.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ECommerce.UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "admin@ecommerce.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEE9Okmcs7JOeWPU2N8YroNmn1t0XwnoHgpTY65QWw35cZJcwIVJUT0OymICu8nXTzQ==",
                    Role = "Admin"
                }
            );
        }
    }
}