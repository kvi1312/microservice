namespace Basket.API.Entities;

public class Cart
{
    public string UserName { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(x => x.ItemPrice * x.Quantity);
    public Cart()
    {
    }
    public Cart(string userName)
    {
        UserName = userName;
    }

    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

    public string EmailAddress { get; set; } = string.Empty;

    public string? JobId { get; set; } = string.Empty;
}