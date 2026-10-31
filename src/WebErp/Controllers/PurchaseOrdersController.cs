using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebErp.Data;
using WebErp.Models;

namespace WebErp.Controllers;

public class PurchaseOrdersController : Controller
{
    private readonly AppDbContext _db;

    public PurchaseOrdersController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var orders = await _db.PurchaseOrders
            .Include(order => order.Vendor)
            .Include(order => order.Lines)
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.Number)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await FindAsync(id);
        return order == null ? NotFound() : View(order);
    }

    public async Task<IActionResult> Create()
    {
        var model = new PurchaseOrderFormModel();
        await FillVendorsAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PurchaseOrderFormModel model)
    {
        await SaveAsync(model, existing: null);
        if (!ModelState.IsValid)
        {
            await FillVendorsAsync(model);
            return View("Form", model);
        }

        TempData["Message"] = "Purchase order saved.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var order = await FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var model = new PurchaseOrderFormModel
        {
            Id = order.Id,
            Number = order.Number,
            VendorId = order.VendorId,
            OrderDate = order.OrderDate,
            ExpectedDate = order.ExpectedDate,
            Status = order.Status,
            Lines = order.Lines.Select(line => new LineFormModel
            {
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            }).ToList()
        };
        await FillVendorsAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PurchaseOrderFormModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var order = await FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        await SaveAsync(model, order);
        if (!ModelState.IsValid)
        {
            model.Number = order.Number;
            await FillVendorsAsync(model);
            return View("Form", model);
        }

        TempData["Message"] = "Purchase order saved.";
        return RedirectToAction(nameof(Details), new { id = order.Id });
    }

    private async Task SaveAsync(PurchaseOrderFormModel model, PurchaseOrder? existing)
    {
        if (!PurchaseOrderStatus.All.Contains(model.Status))
        {
            ModelState.AddModelError(nameof(model.Status), "Choose a valid status.");
        }

        if (model.ExpectedDate.Date < model.OrderDate.Date)
        {
            ModelState.AddModelError(nameof(model.ExpectedDate), "Expected date cannot be before the order date.");
        }

        if (!await _db.Vendors.AnyAsync(vendor => vendor.Id == model.VendorId))
        {
            ModelState.AddModelError(nameof(model.VendorId), "Choose a vendor.");
        }

        LineRules.Validate(model.Lines, ModelState);
        if (!ModelState.IsValid)
        {
            return;
        }

        var order = existing ?? new PurchaseOrder();
        order.VendorId = model.VendorId;
        order.OrderDate = model.OrderDate.Date;
        order.ExpectedDate = model.ExpectedDate.Date;
        order.Status = model.Status;

        if (existing == null)
        {
            order.Number = await DocumentNumbers.NextAsync(_db, "PO");
            order.Lines = MapLines(model.Lines);
            _db.PurchaseOrders.Add(order);
        }
        else
        {
            _db.PurchaseOrderLines.RemoveRange(order.Lines);
            order.Lines.Clear();
            foreach (var line in MapLines(model.Lines))
            {
                order.Lines.Add(line);
            }
        }

        await _db.SaveChangesAsync();
        model.Id = order.Id;
    }

    private static List<PurchaseOrderLine> MapLines(IList<LineFormModel> lines) =>
        LineRules.Kept(lines).Select(line => new PurchaseOrderLine
        {
            Description = line.Description.Trim(),
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice
        }).ToList();

    private async Task FillVendorsAsync(PurchaseOrderFormModel model)
    {
        model.Vendors = await _db.Vendors
            .OrderBy(vendor => vendor.Name)
            .Select(vendor => new SelectListItem(vendor.Name, vendor.Id.ToString()))
            .ToListAsync();
    }

    private Task<PurchaseOrder?> FindAsync(int id) =>
        _db.PurchaseOrders
            .Include(order => order.Vendor)
            .Include(order => order.Lines)
            .FirstOrDefaultAsync(order => order.Id == id);
}
