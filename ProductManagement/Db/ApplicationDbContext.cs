using ProductManagement.Product;

namespace ProductManagement.Db;

using Microsoft.EntityFrameworkCore;
using Cart;
using User;
using Role;
using Order;
using Product;
using Token;
using Permission;
using Category;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Token> Token { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithOne(user => user.Order)
            .HasForeignKey<Order>(o => o.UserId);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Product)
            .WithMany()
            .HasForeignKey(o => o.ProductId);

        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne()
            .HasForeignKey(c => c.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId);

        modelBuilder.Entity<Category>()
            .HasIndex(category => category.Name)
            .IsUnique();
        
        modelBuilder.Entity<Category>().HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany();

        modelBuilder.Entity<User>()
            .HasMany(u => u.Tokens)
            .WithOne(token => token.User)
            .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany();

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        var user = "Anonymous";

        foreach (var entityEntry in ChangeTracker.Entries<BaseEntity>()
                     .Where(e => e.State.Equals(EntityState.Added) || e.State.Equals(EntityState.Modified)))
        {
            if (entityEntry.State.Equals(EntityState.Added))
            {
                entityEntry.Entity.CreatedAt = DateTime.UtcNow;
                entityEntry.Entity.CreatedBy = user;
            }
            entityEntry.Entity.ModifiedAt = DateTime.UtcNow;
            entityEntry.Entity.ModifiedBy = user;
        }
        return base.SaveChanges();
    }
}