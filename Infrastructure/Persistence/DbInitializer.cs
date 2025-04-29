
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Persistence;

public class DbInitializer(SaviorDbContext context) : IDbInitializer
{
    public async Task InitializerAsync()
    {
        try
        {
            Console.WriteLine("Starting DB Initialization...");

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var seedingPath = Path.Combine(basePath, "Data", "Seeding");

            // Seed Pharmacies
            if (!context.Set<Pharmacy>().Any())
            {
                Console.WriteLine("Seeding Pharmacies...");
                await SeedEntities<Pharmacy>(context, seedingPath, "pharmacies.json");
            }

            // Seed Medicines
            if (!context.Set<Medicine>().Any())
            {
                Console.WriteLine("Seeding Medicines...");
                await SeedEntities<Medicine>(context, seedingPath, "medicines.json");
            }

            // Seed Delivery Persons
         
            if (!context.Set<DeliveryPerson>().Any())
            {
                Console.WriteLine("Seeding Delivery Persons...");
                var seedingBasePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seeding");
                var deliveryFilePath = Path.Combine(seedingBasePath, "deliveries.json");

                if (File.Exists(deliveryFilePath))
                {
                    var jsonData = await File.ReadAllTextAsync(deliveryFilePath);
                    var deliveryPersons = JsonSerializer.Deserialize<List<DeliveryPerson>>(jsonData);

                    if (deliveryPersons?.Any() == true)
                    {
                        await context.Set<DeliveryPerson>().AddRangeAsync(deliveryPersons);
                        await context.SaveChangesAsync();
                    }
                }
            }

            // Seed Medical Staff (Doctors, Nurses, Assistants)
            if (!context.Set<MedicalStaffMember>().Any(s => s.Role == MedicalRole.DOCTOR))
            {
                Console.WriteLine("Seeding Doctors...");
                await SeedMedicalStaff(context, seedingPath, "doctors.json", MedicalRole.DOCTOR);
            }

            if (!context.Set<MedicalStaffMember>().Any(s => s.Role == MedicalRole.NURSE))
            {
                Console.WriteLine("Seeding Nurses...");
                await SeedMedicalStaff(context, seedingPath, "nurses.json", MedicalRole.NURSE);
            }

            if (!context.Set<MedicalStaffMember>().Any(s => s.Role == MedicalRole.ASSIST))
            {
                Console.WriteLine("Seeding Assistants...");
                await SeedMedicalStaff(context, seedingPath, "assistants.json", MedicalRole.ASSIST);
            }

            // Seed Availability Entries
            await context.SaveChangesAsync();

            // AvailabilityEntries
            if (!context.Set<AvailabilityEntry>().Any())
            {
                Console.WriteLine("Seeding Availability Entries...");
                var availabilityFilePath = Path.Combine(seedingPath, "availability.json");
                var availabilityData = await File.ReadAllTextAsync(availabilityFilePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var availabilityEntries = JsonSerializer.Deserialize<List<AvailabilityEntry>>(availabilityData, options);

                if (availabilityEntries is not null && availabilityEntries.Any())
                {
                    var validEntries = new List<AvailabilityEntry>();

                    foreach (var entry in availabilityEntries)
                    {
                        if (string.IsNullOrEmpty(entry.ToTime) || string.IsNullOrEmpty(entry.FromTime))
                        {
                            Console.WriteLine($"Invalid AvailabilityEntry: Missing time values for MedicalStaffMemberId={entry.MedicalStaffMemberId}");
                            continue;
                        }

                        var staffExists = await context.MedicalStaffMembers
                            .AnyAsync(m => m.Id == entry.MedicalStaffMemberId);

                        if (!staffExists)
                        {
                            Console.WriteLine($"MedicalStaffMember with ID {entry.MedicalStaffMemberId} not found");
                            continue;
                        }

                        validEntries.Add(entry);
                    }

                    await context.Set<AvailabilityEntry>().AddRangeAsync(validEntries);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Seeded {validEntries.Count} Availability Entries.");
                }
            }

            // Seed Orders (Optional)
            if (!context.Orders.Any())
            {
                Console.WriteLine("Seeding Orders...");
                var orders = new List<Order>
                {
                    new Order
                    {
                        UserID = 1,
                        UserLatitude = 30.0444,
                        UserLongitude = 31.2357,
                        OrderDate = DateTime.UtcNow,
                        TotalPrice = 100.00m,
                        OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                MedicineID = 1, // Ensure this ID exists in the Medicines table
                                Quantity = 2,
                                Price = 50.00m
                            }
                        }
                    }
                };

                context.Orders.AddRange(orders);
                await context.SaveChangesAsync();
                Console.WriteLine("Orders seeded.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during DB seeding: {ex.Message}");
        }
    }

    private async Task SeedEntities<T>(SaviorDbContext context, string seedingPath, string fileName) where T : class
    {
        var filePath = Path.Combine(seedingPath, fileName);

        var data = await File.ReadAllTextAsync(filePath);
        var objects = JsonSerializer.Deserialize<List<T>>(data);

        if (objects is not null && objects.Any())
        {
            context.Set<T>().AddRange(objects);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedMedicalStaff(SaviorDbContext context, string seedingPath, string fileName, MedicalRole role)
    {
        var filePath = Path.Combine(seedingPath, fileName);

        try
        {
            // Log the file path for debugging
            Console.WriteLine($"Reading file from path: {filePath}");

            // Read the file content
            var data = await File.ReadAllTextAsync(filePath);
            Console.WriteLine($"File content: {data}"); // Log the file content

            // Deserialize the JSON data
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // Ignore case sensitivity
                AllowTrailingCommas = true // Allow trailing commas in JSON
            };

            var staffMembers = JsonSerializer.Deserialize<List<MedicalStaffMember>>(data, options);

            if (staffMembers is null || !staffMembers.Any())
            {
                throw new InvalidOperationException($"Failed to deserialize data from file: {fileName}");
            }

            foreach (var staff in staffMembers)
            {
                staff.Role = role;

                // Log each staff member's details for debugging
                Console.WriteLine($"Processing staff member: ID={staff.Id}, Name={staff.Name}, Phone={staff.Phone}");

                if (string.IsNullOrEmpty(staff.Name))
                {
                    throw new InvalidOperationException($"Name is missing for staff member with ID: {staff.Id}");
                }

                if (string.IsNullOrEmpty(staff.Phone))
                {
                    throw new InvalidOperationException($"Phone number is missing for staff member: {staff.Name}");
                }
            }

            context.Set<MedicalStaffMember>().AddRange(staffMembers);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding medical staff from file {fileName}: {ex.Message}");

            throw;
        }
    }
}