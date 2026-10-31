using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebErp.Data;
using WebErp.Models;

namespace WebErp.Controllers;

public class VendorsController : Controller
{
    private readonly AppDbContext _db;

    public VendorsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vendors = await _db.Vendors
            .Include(vendor => vendor.PurchaseOrders)
            .ThenInclude(order => order.Lines)
            .OrderBy(vendor => vendor.Name)
            .ToListAsync();
        return View(vendors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var vendor = await FindAsync(id);
        return vendor == null ? NotFound() : View(vendor);
    }

    public IActionResult Create() => View("Form", new Vendor());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,Phone,City,Category")] Vendor vendor)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", vendor);
        }

        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Vendor saved.";
        return RedirectToAction(nameof(Details), new { id = vendor.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vendor = await _db.Vendors.FindAsync(id);
        return vendor == null ? NotFound() : View("Form", vendor);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,City,Category")] Vendor vendor)
    {
        if (id != vendor.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", vendor);
        }

        _db.Update(vendor);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Vendor saved.";
        return RedirectToAction(nameof(Details), new { id = vendor.Id });
    }

    private Task<Vendor?> FindAsync(int id) =>
        _db.Vendors
            .Include(vendor => vendor.PurchaseOrders)
            .ThenInclude(order => order.Lines)
            .FirstOrDefaultAsync(vendor => vendor.Id == id);
}
