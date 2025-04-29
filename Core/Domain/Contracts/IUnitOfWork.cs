using Domain.Models;
using Domain.Models.CartEntities;
using Domain.Models.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Pharmacy> Pharmacies { get; }
        IGenericRepository<Medicine> Medicines { get; }
        IGenericRepository<Supplies> Supplies { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Cart> Carts { get; }
        IGenericRepository<Emergency> Emergencies { get; }
        IGenericRepository<MedicalStaffMember> MedicalStaffMembers { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<DeliveryPerson> DeliveryPersons { get; }
        Task<int> CompleteAsync();
    }
}
