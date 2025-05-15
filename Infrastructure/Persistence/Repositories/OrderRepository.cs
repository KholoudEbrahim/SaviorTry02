using Domain.Contracts;
using Domain.Models;
using Domain.Models.OrderEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(SaviorDbContext context) : base(context) { }

        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<Order>> GetPastOrdersAsync(int userID)
        {
            return await _dbSet
                .Where(o => o.UserID == userID)
                .Include(o => o.OrderItems)
                 .ThenInclude(oi => oi.Medicine)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetAllUsersOrdersAsync()
        {
            return await _dbSet              
                .Include(o => o.OrderItems)
                 .ThenInclude(oi => oi.Medicine)
                .ToListAsync();
        }

    }

}
