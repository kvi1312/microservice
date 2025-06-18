using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shared.Enum.Order;

namespace Shared.DTOS.Order;

public class OrderDto
{
    public string UserName { get; set; }
    public Guid DocumentNo { get; set; } = Guid.NewGuid();
    public decimal TotalPrice { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string ShippingAddress { get; set; }
    public string InvoiceAddress { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public long Id { get; set; }
    public OrderStatus OrderStatus { get; set; }
}
