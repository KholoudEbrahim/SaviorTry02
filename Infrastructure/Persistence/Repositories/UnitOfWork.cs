using Domain.Contracts;
using Domain.Models;
using Domain.Models.CartEntities;
using Domain.Models.OrderEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SaviorDbContext _context; // Use SaviorDbContext instead of DbContext

        public IGenericRepository<Pharmacy> Pharmacies { get; private set; }
        public IGenericRepository<Medicine> Medicines { get; private set; }
        public IGenericRepository<Supplies> Supplies { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }
        public IGenericRepository<Cart> Carts { get; private set; }
        public IGenericRepository<Emergency> Emergencies { get; private set; }
        public IGenericRepository<MedicalStaffMember> MedicalStaffMembers { get; private set; }
        public IGenericRepository<User> Users { get; }

        public UnitOfWork(SaviorDbContext context) // Change parameter type to SaviorDbContext
        {
            _context = context;
            Pharmacies = new GenericRepository<Pharmacy>(_context);
            Medicines = new GenericRepository<Medicine>(_context);
            Supplies = new GenericRepository<Supplies>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
            Carts = new GenericRepository<Cart>(_context);
            Emergencies = new GenericRepository<Emergency>(_context);
            MedicalStaffMembers = new GenericRepository<MedicalStaffMember>(_context);
            Users = new GenericRepository<User>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
