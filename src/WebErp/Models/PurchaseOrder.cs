using System.ComponentModel.DataAnnotations;

namespace WebErp.Models;

public class PurchaseOrder
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Number { get; set; } = "";

    public int VendorId { get; set; }

    public Vendor? Vendor { get; set; }

    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime ExpectedDate { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = PurchaseOrderStatus.Draft;

    public ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
}

public class PurchaseOrderLine
{
    public int Id { get; set; }

    public int PurchaseOrderId { get; set; }

    public PurchaseOrder? PurchaseOrder { get; set; }

    [Required, MaxLength(160)]
    public string Description { get; set; } = "";

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
