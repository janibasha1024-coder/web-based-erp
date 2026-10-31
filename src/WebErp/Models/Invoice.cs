using System.ComponentModel.DataAnnotations;

namespace WebErp.Models;

public class Invoice
{
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Number { get; set; } = "";

    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [DataType(DataType.Date)]
    public DateTime IssueDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = InvoiceStatus.Draft;

    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}

public class InvoiceLine
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    [Required, MaxLength(160)]
    public string Description { get; set; } = "";

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
