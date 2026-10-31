using System.ComponentModel.DataAnnotations;

namespace WebErp.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, MaxLength(160)]
    public string Email { get; set; } = "";

    [MaxLength(40)]
    public string? Phone { get; set; }

    [Required, MaxLength(80)]
    public string City { get; set; } = "";

    [Required, MaxLength(40)]
    public string State { get; set; } = "";

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
