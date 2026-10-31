using Microsoft.EntityFrameworkCore;
using WebErp.Models;

namespace WebErp.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Customers.AnyAsync())
        {
            return;
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        var today = DateTime.Today;

        var maple = Customer("Maple Street Books", "orders@maplestreetbooks.example", "503-555-0142", "Portland", "OR");
        var cedar = Customer("Cedar & Co. Cafe", "hello@cedarandco.example", "206-555-0188", "Seattle", "WA");
        var bright = Customer("Bright Path Clinic", "purchasing@brightpath.example", "303-555-0164", "Denver", "CO");
        var school = Customer("Harborview School", "office@harborviewschool.example", "253-555-0190", "Tacoma", "WA");
        var lumen = Customer("Lumen Print Studio", "studio@lumenprint.example", "512-555-0177", "Austin", "TX");
        var oak = Customer("Oak & Iron Hardware", "desk@oakandiron.example", "208-555-0133", "Boise", "ID");

        var paper = Vendor("Pacific Paper Co.", "sales@pacificpaper.example", "503-555-0101", "Portland", "Paper");
        var packaging = Vendor("Summit Packaging", "orders@summitpack.example", "206-555-0108", "Seattle", "Packaging");
        var janitorial = Vendor("Northwind Janitorial", "accounts@northwindjan.example", "425-555-0114", "Bellevue", "Supplies");
        var furniture = Vendor("Cascade Office Furniture", "hello@cascadeoffice.example", "253-555-0120", "Tacoma", "Furniture");
        var ink = Vendor("Metro Ink Supply", "desk@metroink.example", "512-555-0126", "Austin", "Printing");

        db.Customers.AddRange(maple, cedar, bright, school, lumen, oak);
        db.Vendors.AddRange(paper, packaging, janitorial, furniture, ink);
        await db.SaveChangesAsync();

        db.Invoices.AddRange(
            Invoice("INV-1001", maple, today.AddDays(-12), today.AddDays(18), InvoiceStatus.Sent,
                ("Copy paper", 8, 42.50m), ("Binder clips", 12, 6.25m)),
            Invoice("INV-1002", cedar, today.AddDays(-30), today.AddDays(-5), InvoiceStatus.Paid,
                ("Napkins", 6, 28.00m), ("Coffee sleeves", 10, 14.50m)),
            Invoice("INV-1003", bright, today.AddDays(-40), today.AddDays(-10), InvoiceStatus.Overdue,
                ("Exam table paper", 20, 18.75m), ("Gloves", 15, 9.40m)),
            Invoice("INV-1004", school, today.AddDays(-6), today.AddDays(24), InvoiceStatus.Sent,
                ("Notebooks", 40, 3.85m), ("Pencils", 25, 4.10m)),
            Invoice("INV-1005", lumen, today, today.AddDays(30), InvoiceStatus.Draft,
                ("Card stock", 30, 11.20m)),
            Invoice("INV-1006", oak, today.AddDays(-20), today.AddDays(-2), InvoiceStatus.Paid,
                ("Packing tape", 24, 3.15m), ("Mailers", 8, 16.00m)),
            Invoice("INV-1007", maple, today.AddDays(-50), today.AddDays(-15), InvoiceStatus.Overdue,
                ("Toner", 2, 89.00m)),
            Invoice("INV-1008", cedar, today.AddDays(-1), today.AddDays(14), InvoiceStatus.Draft,
                ("Cups", 4, 32.00m)));

        db.PurchaseOrders.AddRange(
            Order("PO-2001", paper, today.AddDays(-8), today.AddDays(6), PurchaseOrderStatus.Ordered,
                ("Letter paper", 20, 36.00m), ("Card stock", 15, 9.50m)),
            Order("PO-2002", packaging, today.AddDays(-25), today.AddDays(-5), PurchaseOrderStatus.Received,
                ("Mailers", 10, 14.00m), ("Tape", 40, 2.40m)),
            Order("PO-2003", janitorial, today, today.AddDays(10), PurchaseOrderStatus.Draft,
                ("Cleaner", 12, 8.75m)),
            Order("PO-2004", furniture, today.AddDays(-3), today.AddDays(20), PurchaseOrderStatus.Ordered,
                ("Task chair", 4, 180.00m)),
            Order("PO-2005", ink, today.AddDays(-15), today.AddDays(-1), PurchaseOrderStatus.Cancelled,
                ("Cyan ink", 6, 22.00m)),
            Order("PO-2006", paper, today.AddDays(-40), today.AddDays(-12), PurchaseOrderStatus.Received,
                ("Notebooks", 50, 2.90m)));

        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static Customer Customer(string name, string email, string phone, string city, string state) =>
        new() { Name = name, Email = email, Phone = phone, City = city, State = state };

    private static Vendor Vendor(string name, string email, string phone, string city, string category) =>
        new() { Name = name, Email = email, Phone = phone, City = city, Category = category };

    private static Invoice Invoice(
        string number,
        Customer customer,
        DateTime issued,
        DateTime due,
        string status,
        params (string Description, decimal Quantity, decimal UnitPrice)[] lines) =>
        new()
        {
            Number = number,
            CustomerId = customer.Id,
            IssueDate = issued,
            DueDate = due,
            Status = status,
            Lines = lines.Select(line => new InvoiceLine
            {
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            }).ToList()
        };

    private static PurchaseOrder Order(
        string number,
        Vendor vendor,
        DateTime ordered,
        DateTime expected,
        string status,
        params (string Description, decimal Quantity, decimal UnitPrice)[] lines) =>
        new()
        {
            Number = number,
            VendorId = vendor.Id,
            OrderDate = ordered,
            ExpectedDate = expected,
            Status = status,
            Lines = lines.Select(line => new PurchaseOrderLine
            {
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            }).ToList()
        };
}
