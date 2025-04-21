
using ECommerceShop.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShop.Data;
public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductInteraction> ProductInteractions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite primary key for ProductInteractions
        modelBuilder.Entity<ProductInteraction>()
            .HasKey(pi => pi.InteractionId);

        // Configure the many-to-many relationships
        modelBuilder.Entity<ProductInteraction>()
            .HasOne(pi => pi.User)
            .WithMany(u => u.ProductInteractions)
            .HasForeignKey(pi => pi.UserId);

        modelBuilder.Entity<ProductInteraction>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.ProductInteractions)
            .HasForeignKey(pi => pi.ProductId);
    }
}