using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebErp.Models;

public class LineFormModel
{
    [MaxLength(160)]
    public string Description { get; set; } = "";

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public bool IsBlank =>
        string.IsNullOrWhiteSpace(Description) && Quantity == 0 && UnitPrice == 0;
}

public class InvoiceFormModel
{
    public int Id { get; set; }

    public string? Number { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choose a customer.")]
    public int CustomerId { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime IssueDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

    [Required]
    public string Status { get; set; } = InvoiceStatus.Draft;

    public List<LineFormModel> Lines { get; set; } = [new()];

    [ValidateNever]
    public List<SelectListItem> Customers { get; set; } = [];
}

public class PurchaseOrderFormModel
{
    public int Id { get; set; }

    public string? Number { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choose a vendor.")]
    public int VendorId { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime OrderDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime ExpectedDate { get; set; } = DateTime.Today.AddDays(14);

    [Required]
    public string Status { get; set; } = PurchaseOrderStatus.Draft;

    public List<LineFormModel> Lines { get; set; } = [new()];

    [ValidateNever]
    public List<SelectListItem> Vendors { get; set; } = [];
}

public class DashboardViewModel
{
    public int CustomerCount { get; set; }
    public int VendorCount { get; set; }
    public int OpenInvoiceCount { get; set; }
    public decimal OpenInvoiceTotal { get; set; }
    public int OpenPurchaseOrderCount { get; set; }
    public decimal OpenPurchaseOrderTotal { get; set; }
    public List<Invoice> RecentInvoices { get; set; } = [];
    public List<PurchaseOrder> RecentPurchaseOrders { get; set; } = [];
}
