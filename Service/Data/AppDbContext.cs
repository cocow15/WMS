using Microsoft.EntityFrameworkCore;
using ApplicationTest.Entities;

namespace ApplicationTest.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ExternalAuthToken> ExternalAuthTokens => Set<ExternalAuthToken>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("public");
        b.Entity<User>().ToTable("users", "auth");
        b.Entity<ExternalAuthToken>().ToTable("external_auth_tokens", "auth");
        b.Entity<Brand>().ToTable("brands", "catalog");
        b.Entity<Category>().ToTable("categories", "catalog");
        b.Entity<Product>().ToTable("products", "catalog");

        b.Entity<User>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.ExternalAuthToken)
             .WithOne(t => t.User)
             .HasForeignKey<ExternalAuthToken>(t => t.UserId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Brand>().HasIndex(x => x.Name).IsUnique();
        b.Entity<Category>().HasIndex(x => x.Name).IsUnique();

        b.Entity<Product>(e =>
        {
            e.HasKey(p => p.ProductId);
            e.HasIndex(p => p.Sku).IsUnique();
            e.HasOne(p => p.Brand).WithMany(bx => bx.Products).HasForeignKey(p => p.BrandId);
            e.HasOne(p => p.Category).WithMany(cx => cx.Products).HasForeignKey(p => p.CategoryId);
            e.HasOne(p => p.User).WithMany(u => u.Products).HasForeignKey(p => p.UserId);
        });
    }
}