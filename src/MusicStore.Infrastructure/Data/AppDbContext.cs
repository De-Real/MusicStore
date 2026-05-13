using Microsoft.EntityFrameworkCore;
using MusicStore.Application.Common;
using MusicStore.Domain.Common;
using MusicStore.Domain.Entities;

namespace MusicStore.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly IDomainEventDispatcher _dispatcher;

    public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        if (aggregatesWithEvents.Count > 0)
            await _dispatcher.DispatchAndClearAsync(aggregatesWithEvents, cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.TotalSpent).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.OwnsOne(e => e.TotalPrice, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("TotalAmount")
                     .HasPrecision(18, 2);
                money.Property(m => m.Currency)
                     .HasColumnName("TotalCurrency")
                     .HasMaxLength(3)
                     .HasDefaultValue("UAH");
            });

            entity.OwnsOne(e => e.ShippingAddress, addr =>
            {
                addr.Property(a => a.Street)
                    .HasColumnName("ShippingAddressStreet")
                    .HasMaxLength(200);
                addr.Property(a => a.City)
                    .HasColumnName("ShippingAddressCity")
                    .HasMaxLength(100);
                addr.Property(a => a.Country)
                    .HasColumnName("ShippingAddressCountry")
                    .HasMaxLength(100);
                addr.Property(a => a.PostalCode)
                    .HasColumnName("ShippingAddressPostalCode")
                    .HasMaxLength(20);
            });

            entity.Navigation(e => e.OrderItems)
                  .HasField("_orderItems")
                  .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalUnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TierDiscountPercent).HasPrecision(5, 2);
            entity.Property(e => e.BulkDiscountPercent).HasPrecision(5, 2);
            entity.Property(e => e.PromoDiscountPercent).HasPrecision(5, 2);
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DiscountPercent).HasPrecision(5, 2);
            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);
        });
    }
}
