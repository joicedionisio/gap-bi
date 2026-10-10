using GapBi.Api.Data;
using GapBi.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GapBi.Api.Services;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var carts = await _context.Carts.ToListAsync();

        var totalOrders = carts.Count;
        var grossRevenue = carts.Sum(c => c.Total);
        var netRevenue = carts.Sum(c => c.DiscountedTotal);
        var totalDiscount = grossRevenue - netRevenue;
        var discountPercentage = grossRevenue > 0 ? (totalDiscount / grossRevenue) * 100 : 0;
        var averageTicket = totalOrders > 0 ? netRevenue / totalOrders : 0;

        var revenueByCategory = await _context.CartItems
            .Include(ci => ci.Product)
            .GroupBy(ci => ci.Product.Category)
            .Select(g => new CategoryRevenueDto
            {
                Category = g.Key,
                NetRevenue = g.Sum(ci => ci.DiscountedPrice * ci.Quantity)
            })
            .OrderByDescending(c => c.NetRevenue)
            .ToListAsync();

        var topCustomers = await _context.Carts
            .Include(c => c.Customer)
            .GroupBy(c => new { c.CustomerId, c.Customer.FirstName, c.Customer.LastName })
            .Select(g => new TopCustomerDto
            {
                FullName = g.Key.FirstName + " " + g.Key.LastName,
                NetRevenue = g.Sum(c => c.DiscountedTotal)
            })
            .OrderByDescending(c => c.NetRevenue)
            .Take(5)
            .ToListAsync();

        var lastSync = await _context.SyncLogs
            .Where(s => s.Success)
            .OrderByDescending(s => s.SyncedAt)
            .FirstOrDefaultAsync();

        return new DashboardDto
        {
            TotalOrders = totalOrders,
            GrossRevenue = grossRevenue,
            NetRevenue = netRevenue,
            TotalDiscount = totalDiscount,
            DiscountPercentage = Math.Round(discountPercentage, 2),
            AverageTicket = Math.Round(averageTicket, 2),
            RevenueByCategory = revenueByCategory,
            TopCustomers = topCustomers,
            LastSyncAt = lastSync?.SyncedAt
        };
    }
}