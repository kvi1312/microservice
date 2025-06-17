namespace Shared.DTOS.Order;

public class CreateOrderDto
{
    public string UserNAme { get; set; }
    public decimal TotalPrice { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string ShippingAddress { get; set; }
    private string _invoiceAddress = string.Empty;
    public string InvoiceAddress { get => _invoiceAddress; set => _invoiceAddress = value ?? ShippingAddress; }
}
