namespace GapBi.Api.DTOs;

public class DashboardDto
{
    public int TotalOrders { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal AverageTicket { get; set; }
    public List<CategoryRevenueDto> RevenueByCategory { get; set; } = new();
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
    public DateTime? LastSyncAt { get; set; }
}

public class CategoryRevenueDto
{
    public string Category { get; set; } = string.Empty;
    public decimal NetRevenue { get; set; }
}

public class TopCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public decimal NetRevenue { get; set; }
}