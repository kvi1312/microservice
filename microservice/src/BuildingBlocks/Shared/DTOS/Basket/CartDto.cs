namespace Shared.DTOS.Basket;

public class CartDto
{
    public string UserName { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(x => x.ItemPrice * x.Quantity);
    public string EmailAddress { get; set; } = string.Empty;
    public CartDto()
    {
    }
    public CartDto(string userName)
    {
        UserName = userName;
    }
}
