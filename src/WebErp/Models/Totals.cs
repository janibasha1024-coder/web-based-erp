namespace WebErp.Models;

public static class Totals
{
    public static decimal Amount(decimal quantity, decimal unitPrice) => quantity * unitPrice;

    public static decimal For(Invoice invoice) =>
        invoice.Lines.Sum(line => Amount(line.Quantity, line.UnitPrice));

    public static decimal For(PurchaseOrder order) =>
        order.Lines.Sum(line => Amount(line.Quantity, line.UnitPrice));

    public static decimal Outstanding(Customer customer) =>
        customer.Invoices.Where(invoice => InvoiceStatus.IsOpen(invoice.Status)).Sum(For);

    public static decimal OpenOrders(Vendor vendor) =>
        vendor.PurchaseOrders.Where(order => PurchaseOrderStatus.IsOpen(order.Status)).Sum(For);
}
