namespace GapBi.Api.Models;

public class Cart
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public decimal DiscountedTotal { get; set; }

    public Customer Customer { get; set; } = null!;
    public List<CartItem> Items { get; set; } = new();
}