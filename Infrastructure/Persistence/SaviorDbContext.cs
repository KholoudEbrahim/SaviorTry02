using Domain.Models;
using Domain.Models.CartEntities;
using Domain.Models.OrderEntities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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

        // DbSets
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
        public DbSet<AvailabilityEntry> AvailabilityEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SaviorDbContext).Assembly);

            // Configure Supplies
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

            // Configure Cart and CartItem
            modelBuilder.Entity<Cart>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.MedicineID, ci.CartId });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Medicine)
            .WithMany()
                .HasForeignKey(ci => ci.MedicineID);

            modelBuilder.Entity<CartItem>()
              .Property(ci => ci.Price)
              .HasColumnType("decimal(18, 2)");
            // Configure Emergency
            modelBuilder.Entity<Emergency>()
                .HasKey(e => e.Id);

            // Configure MedicalStaffMember
            modelBuilder.Entity<MedicalStaffMember>()
                .HasKey(msm => msm.Id);

            modelBuilder.Entity<MedicalStaffMember>()
                .Property(msm => msm.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<MedicalStaffMember>().ToTable("MedicalStaffMembers");

            // Configure AvailabilityEntry
        
            modelBuilder.Entity<AvailabilityEntry>()
     .HasOne(ae => ae.MedicalStaffMember)
     .WithMany(msm => msm.Availability)
     .HasForeignKey(ae => ae.MedicalStaffMemberId);

            modelBuilder.Entity<AvailabilityEntry>()
    .Property(ae => ae.Day)
    .IsRequired();

            modelBuilder.Entity<AvailabilityEntry>()
                .Property(ae => ae.FromTime)
                .IsRequired();

            modelBuilder.Entity<AvailabilityEntry>()
                .Property(ae => ae.ToTime)
                .IsRequired();
            // Configure Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.ShippingPrice).HasColumnType("decimal(18,2)");
                entity.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");

                // Relationship with OrderItems
                entity.HasMany(o => o.OrderItems) 
                      .WithOne(oi => oi.Order)   
                      .HasForeignKey(oi => oi.OrderID) 
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            // Configure OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.Price).HasColumnType("decimal(18,2)");

                // Relationship with Medicine (if exists)
                entity.HasOne(oi => oi.Medicine)
                      .WithMany()
                      .HasForeignKey(oi => oi.MedicineID);
            });

          
        }
    }

}
