using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContext;

    public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddOrderAsync(OrderHeader orderHeader)
    {
        try
        {
            await using var db = new ApplicationDbContext(_dbContext);
            await db.OrderHeaders.AddAsync(orderHeader);
            await db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task UpdateOrderPaymentStatusAsync(int orderHeaderId, bool isPaid)
    {
        await using var _db = new ApplicationDbContext(_dbContext);

        var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(oh => oh.OrderHeaderId == orderHeaderId);

        if (orderHeader is not null)
        {
            orderHeader.PaymentStatus = isPaid;
            await _db.SaveChangesAsync();
        }
    }
}