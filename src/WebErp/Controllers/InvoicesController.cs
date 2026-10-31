using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebErp.Data;
using WebErp.Models;

namespace WebErp.Controllers;

public class InvoicesController : Controller
{
    private readonly AppDbContext _db;

    public InvoicesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var invoices = await _db.Invoices
            .Include(invoice => invoice.Customer)
            .Include(invoice => invoice.Lines)
            .OrderByDescending(invoice => invoice.IssueDate)
            .ThenByDescending(invoice => invoice.Number)
            .ToListAsync();
        return View(invoices);
    }

    public async Task<IActionResult> Details(int id)
    {
        var invoice = await FindAsync(id);
        return invoice == null ? NotFound() : View(invoice);
    }

    public async Task<IActionResult> Create()
    {
        var model = new InvoiceFormModel();
        await FillCustomersAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceFormModel model)
    {
        await SaveAsync(model, existing: null);
        if (!ModelState.IsValid)
        {
            await FillCustomersAsync(model);
            return View("Form", model);
        }

        TempData["Message"] = "Invoice saved.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var invoice = await FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        var model = new InvoiceFormModel
        {
            Id = invoice.Id,
            Number = invoice.Number,
            CustomerId = invoice.CustomerId,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            Lines = invoice.Lines.Select(line => new LineFormModel
            {
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            }).ToList()
        };
        await FillCustomersAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InvoiceFormModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var invoice = await FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        await SaveAsync(model, invoice);
        if (!ModelState.IsValid)
        {
            model.Number = invoice.Number;
            await FillCustomersAsync(model);
            return View("Form", model);
        }

        TempData["Message"] = "Invoice saved.";
        return RedirectToAction(nameof(Details), new { id = invoice.Id });
    }

    private async Task SaveAsync(InvoiceFormModel model, Invoice? existing)
    {
        if (!InvoiceStatus.All.Contains(model.Status))
        {
            ModelState.AddModelError(nameof(model.Status), "Choose a valid status.");
        }

        if (model.DueDate.Date < model.IssueDate.Date)
        {
            ModelState.AddModelError(nameof(model.DueDate), "Due date cannot be before the issue date.");
        }

        if (!await _db.Customers.AnyAsync(customer => customer.Id == model.CustomerId))
        {
            ModelState.AddModelError(nameof(model.CustomerId), "Choose a customer.");
        }

        LineRules.Validate(model.Lines, ModelState);
        if (!ModelState.IsValid)
        {
            return;
        }

        var invoice = existing ?? new Invoice();
        invoice.CustomerId = model.CustomerId;
        invoice.IssueDate = model.IssueDate.Date;
        invoice.DueDate = model.DueDate.Date;
        invoice.Status = model.Status;

        if (existing == null)
        {
            invoice.Number = await DocumentNumbers.NextAsync(_db, "INV");
            invoice.Lines = MapLines(model.Lines);
            _db.Invoices.Add(invoice);
        }
        else
        {
            _db.InvoiceLines.RemoveRange(invoice.Lines);
            invoice.Lines.Clear();
            foreach (var line in MapLines(model.Lines))
            {
                invoice.Lines.Add(line);
            }
        }

        await _db.SaveChangesAsync();
        model.Id = invoice.Id;
    }

    private static List<InvoiceLine> MapLines(IList<LineFormModel> lines) =>
        LineRules.Kept(lines).Select(line => new InvoiceLine
        {
            Description = line.Description.Trim(),
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice
        }).ToList();

    private async Task FillCustomersAsync(InvoiceFormModel model)
    {
        model.Customers = await _db.Customers
            .OrderBy(customer => customer.Name)
            .Select(customer => new SelectListItem(customer.Name, customer.Id.ToString()))
            .ToListAsync();
    }

    private Task<Invoice?> FindAsync(int id) =>
        _db.Invoices
            .Include(invoice => invoice.Customer)
            .Include(invoice => invoice.Lines)
            .FirstOrDefaultAsync(invoice => invoice.Id == id);
}
