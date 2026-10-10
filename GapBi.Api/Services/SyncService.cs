using GapBi.Api.Data;
using GapBi.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GapBi.Api.Services;

public class SyncService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public SyncService(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task SyncAsync()
    {
        var syncLog = new SyncLog { SyncedAt = DateTime.UtcNow };
        try
        {
            await SyncProductsAsync();
            await SyncCustomersAsync();
            await SyncCartsAsync();

            syncLog.Success = true;
        }
        catch (Exception ex)
        {
            syncLog.Success = false;
            syncLog.ErrorMessage = ex.Message;
            throw;
        }
        finally
        {
            _context.SyncLogs.Add(syncLog);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SyncProductsAsync()
    {
        var response = await _httpClient.GetStringAsync("https://dummyjson.com/products?limit=100");
        var json = JsonDocument.Parse(response);
        var products = json.RootElement.GetProperty("products");

        foreach (var p in products.EnumerateArray())
        {
            var id = p.GetProperty("id").GetInt32();
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
            {
                _context.Products.Add(new Product
                {
                    Id = id,
                    Title = p.GetProperty("title").GetString() ?? "",
                    Category = p.GetProperty("category").GetString() ?? "",
                    Price = p.GetProperty("price").GetDecimal()
                });
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SyncCustomersAsync()
    {
        var response = await _httpClient.GetStringAsync("https://dummyjson.com/users?limit=100");
        var json = JsonDocument.Parse(response);
        var users = json.RootElement.GetProperty("users");

        foreach (var u in users.EnumerateArray())
        {
            var id = u.GetProperty("id").GetInt32();
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null)
            {
                _context.Customers.Add(new Customer
                {
                    Id = id,
                    FirstName = u.GetProperty("firstName").GetString() ?? "",
                    LastName = u.GetProperty("lastName").GetString() ?? "",
                    Email = u.GetProperty("email").GetString() ?? ""
                });
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SyncCartsAsync()
    {
        var response = await _httpClient.GetStringAsync("https://dummyjson.com/carts?limit=100");
        var json = JsonDocument.Parse(response);
        var carts = json.RootElement.GetProperty("carts");

        foreach (var c in carts.EnumerateArray())
        {
            var id = c.GetProperty("id").GetInt32();
            var existing = await _context.Carts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
            {
                var cart = new Cart
                {
                    Id = id,
                    CustomerId = c.GetProperty("userId").GetInt32(),
                    Total = c.GetProperty("total").GetDecimal(),
                    DiscountedTotal = c.GetProperty("discountedTotal").GetDecimal()
                };

                var items = c.GetProperty("products");
                foreach (var item in items.EnumerateArray())
                {
                    cart.Items.Add(new CartItem
                    {
                        ProductId = item.GetProperty("id").GetInt32(),
                        Quantity = item.GetProperty("quantity").GetInt32(),
                        Price = item.GetProperty("price").GetDecimal(),
                        DiscountedPrice = item.GetProperty("discountedPrice").GetDecimal()
                    });
                }
                _context.Carts.Add(cart);
            }
        }
        await _context.SaveChangesAsync();
    }
}