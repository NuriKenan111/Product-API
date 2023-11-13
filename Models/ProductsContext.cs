using Microsoft.EntityFrameworkCore;

namespace ProductApi.Models;

public class ProductsContext : DbContext
{
    public DbSet<Product>? Products { get; set; }
    public ProductsContext(DbContextOptions<ProductsContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 1, ProductName = "Iphone 14", Price = 700, IsActive = true });
        modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 2, ProductName = "Iphone 15", Price = 800, IsActive = true });
        modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 3, ProductName = "Iphone 16", Price = 900, IsActive = false });
        modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 4, ProductName = "Iphone 17", Price = 1000, IsActive = true });
        modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 5, ProductName = "Iphone 18", Price = 1100, IsActive = true });
    }
}