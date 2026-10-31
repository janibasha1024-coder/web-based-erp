using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebErp.Models;

namespace WebErp.Data;

public static class LineRules
{
    public static void Validate(IList<LineFormModel> lines, ModelStateDictionary modelState)
    {
        var kept = lines.Where(line => !line.IsBlank).ToList();
        if (kept.Count == 0)
        {
            modelState.AddModelError(string.Empty, "Add at least one line.");
            return;
        }

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (line.IsBlank)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(line.Description))
            {
                modelState.AddModelError($"Lines[{i}].Description", "Enter a description.");
            }

            if (line.Quantity <= 0)
            {
                modelState.AddModelError($"Lines[{i}].Quantity", "Quantity must be greater than zero.");
            }

            if (line.UnitPrice < 0)
            {
                modelState.AddModelError($"Lines[{i}].UnitPrice", "Price cannot be negative.");
            }
        }
    }

    public static List<LineFormModel> Kept(IList<LineFormModel> lines) =>
        lines.Where(line => !line.IsBlank).ToList();
}
