using Domain.Models;
using Domain.Models.CartEntities;
using Domain.Models.OrderEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class SaviorDbContext : DbContext
    {
        public SaviorDbContext(DbContextOptions<SaviorDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Supplies> Supplies { get; set; }
        public DbSet<Order> Orders { get; set; } 
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DbSet<Emergency> Emergencies { get; set; }
        public DbSet<MedicalStaffMember> MedicalStaffMembers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SaviorDbContext).Assembly);

      
            modelBuilder.Entity<Supplies>()
                .HasKey(s => new { s.PharmacyId, s.MedicineId });

            modelBuilder.Entity<Supplies>()
           .HasOne(s => s.Medicine)
           .WithMany(m => m.Supplies)
           .HasForeignKey(s => s.MedicineId);

            modelBuilder.Entity<Supplies>()
                .HasOne(s => s.Pharmacy)
                .WithMany(p => p.Supplies)
                .HasForeignKey(s => s.PharmacyId);

            // Configure Cart entity
            modelBuilder.Entity<Cart>()
                .HasKey(c => c.Id); 

            // Configure CartItem entity
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.MedicineID, ci.CartId }); 

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);



            modelBuilder.Entity<Emergency>()
         .HasKey(e => e.Id);

            modelBuilder.Entity<MedicalStaffMember>()
                .HasKey(msm => msm.Id);

            modelBuilder.Entity<AvailabilityEntry>()
             .HasKey(ae => ae.Id);

            modelBuilder.Entity<AvailabilityEntry>()
                .HasOne(ae => ae.MedicalStaffMember)
                .WithMany(msm => msm.Availability)
                .HasForeignKey(ae => ae.MedicalStaffMemberId);


        }
    }
    
}
