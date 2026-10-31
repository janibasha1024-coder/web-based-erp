using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebErp.Data;
using WebErp.Models;

namespace WebErp.Controllers;

public class CustomersController : Controller
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var customers = await _db.Customers
            .Include(customer => customer.Invoices)
            .ThenInclude(invoice => invoice.Lines)
            .OrderBy(customer => customer.Name)
            .ToListAsync();
        return View(customers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await FindAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    public IActionResult Create() => View("Form", new Customer());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,Phone,City,State")] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", customer);
        }

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Customer saved.";
        return RedirectToAction(nameof(Details), new { id = customer.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        return customer == null ? NotFound() : View("Form", customer);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,City,State")] Customer customer)
    {
        if (id != customer.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", customer);
        }

        _db.Update(customer);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Customer saved.";
        return RedirectToAction(nameof(Details), new { id = customer.Id });
    }

    private Task<Customer?> FindAsync(int id) =>
        _db.Customers
            .Include(customer => customer.Invoices)
            .ThenInclude(invoice => invoice.Lines)
            .FirstOrDefaultAsync(customer => customer.Id == id);
}
