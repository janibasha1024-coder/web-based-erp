namespace WebErp.Models;

public static class InvoiceStatus
{
    public const string Draft = "Draft";
    public const string Sent = "Sent";
    public const string Paid = "Paid";
    public const string Overdue = "Overdue";

    public static readonly string[] All = [Draft, Sent, Paid, Overdue];

    public static bool IsOpen(string status) => status is Sent or Overdue;
}

public static class PurchaseOrderStatus
{
    public const string Draft = "Draft";
    public const string Ordered = "Ordered";
    public const string Received = "Received";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = [Draft, Ordered, Received, Cancelled];

    public static bool IsOpen(string status) => status is Draft or Ordered;
}
