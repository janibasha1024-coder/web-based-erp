using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebErp.Data;
using WebErp.Models;

namespace WebErp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var invoices = await _db.Invoices
            .Include(invoice => invoice.Customer)
            .Include(invoice => invoice.Lines)
            .ToListAsync();
        var orders = await _db.PurchaseOrders
            .Include(order => order.Vendor)
            .Include(order => order.Lines)
            .ToListAsync();

        var openInvoices = invoices.Where(invoice => InvoiceStatus.IsOpen(invoice.Status)).ToList();
        var openOrders = orders.Where(order => PurchaseOrderStatus.IsOpen(order.Status)).ToList();

        var model = new DashboardViewModel
        {
            CustomerCount = await _db.Customers.CountAsync(),
            VendorCount = await _db.Vendors.CountAsync(),
            OpenInvoiceCount = openInvoices.Count,
            OpenInvoiceTotal = openInvoices.Sum(Totals.For),
            OpenPurchaseOrderCount = openOrders.Count,
            OpenPurchaseOrderTotal = openOrders.Sum(Totals.For),
            RecentInvoices = invoices.OrderByDescending(invoice => invoice.IssueDate).ThenByDescending(invoice => invoice.Id).Take(5).ToList(),
            RecentPurchaseOrders = orders.OrderByDescending(order => order.OrderDate).ThenByDescending(order => order.Id).Take(5).ToList()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
