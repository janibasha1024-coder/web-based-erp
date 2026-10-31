using Microsoft.EntityFrameworkCore;
using WebErp.Models;

namespace WebErp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().Property(customer => customer.Name).HasMaxLength(120);
        modelBuilder.Entity<Customer>().Property(customer => customer.Email).HasMaxLength(160);
        modelBuilder.Entity<Customer>().Property(customer => customer.Phone).HasMaxLength(40);
        modelBuilder.Entity<Customer>().Property(customer => customer.City).HasMaxLength(80);
        modelBuilder.Entity<Customer>().Property(customer => customer.State).HasMaxLength(40);

        modelBuilder.Entity<Vendor>().Property(vendor => vendor.Name).HasMaxLength(120);
        modelBuilder.Entity<Vendor>().Property(vendor => vendor.Email).HasMaxLength(160);
        modelBuilder.Entity<Vendor>().Property(vendor => vendor.Phone).HasMaxLength(40);
        modelBuilder.Entity<Vendor>().Property(vendor => vendor.City).HasMaxLength(80);
        modelBuilder.Entity<Vendor>().Property(vendor => vendor.Category).HasMaxLength(40);

        modelBuilder.Entity<Invoice>().Property(invoice => invoice.Number).HasMaxLength(20);
        modelBuilder.Entity<Invoice>().HasIndex(invoice => invoice.Number).IsUnique();
        modelBuilder.Entity<Invoice>().Property(invoice => invoice.Status).HasMaxLength(20);
        modelBuilder.Entity<Invoice>()
            .HasMany(invoice => invoice.Lines)
            .WithOne(line => line.Invoice!)
            .HasForeignKey(line => line.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InvoiceLine>().Property(line => line.Description).HasMaxLength(160);
        modelBuilder.Entity<InvoiceLine>().Property(line => line.Quantity).HasPrecision(18, 2);
        modelBuilder.Entity<InvoiceLine>().Property(line => line.UnitPrice).HasPrecision(18, 2);

        modelBuilder.Entity<PurchaseOrder>().Property(order => order.Number).HasMaxLength(20);
        modelBuilder.Entity<PurchaseOrder>().HasIndex(order => order.Number).IsUnique();
        modelBuilder.Entity<PurchaseOrder>().Property(order => order.Status).HasMaxLength(20);
        modelBuilder.Entity<PurchaseOrder>()
            .HasMany(order => order.Lines)
            .WithOne(line => line.PurchaseOrder!)
            .HasForeignKey(line => line.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseOrderLine>().Property(line => line.Description).HasMaxLength(160);
        modelBuilder.Entity<PurchaseOrderLine>().Property(line => line.Quantity).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrderLine>().Property(line => line.UnitPrice).HasPrecision(18, 2);
    }
}
