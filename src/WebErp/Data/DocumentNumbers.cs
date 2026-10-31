using Microsoft.EntityFrameworkCore;

namespace WebErp.Data;

public static class DocumentNumbers
{
    public static async Task<string> NextAsync(AppDbContext db, string prefix)
    {
        var numbers = prefix == "INV"
            ? await db.Invoices.Select(invoice => invoice.Number).ToListAsync()
            : await db.PurchaseOrders.Select(order => order.Number).ToListAsync();

        var max = prefix == "INV" ? 1000 : 2000;
        foreach (var number in numbers)
        {
            if (number.StartsWith(prefix + "-", StringComparison.Ordinal)
                && int.TryParse(number[(prefix.Length + 1)..], out var value)
                && value > max)
            {
                max = value;
            }
        }

        return $"{prefix}-{max + 1}";
    }
}
