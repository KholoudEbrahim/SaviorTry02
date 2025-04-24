
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


            if (!context.Set<Pharmacy>().Any())
            {
                Console.WriteLine("Seeding Pharmacies..");
                var filePath = Path.Combine(seedingPath, "pharmacies.json");

                var data = await File.ReadAllTextAsync(filePath);
                var objects = JsonSerializer.Deserialize<List<Pharmacy>>(data);

                if (objects is not null && objects.Any())
                {
                    context.Set<Pharmacy>().AddRange(objects);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Pharmacies seeded.");
                }
            }


            if (!context.Set<Medicine>().Any())
            {
                Console.WriteLine("Seeding Medicines...");
                var filePath = Path.Combine(seedingPath, "medicines.json");

                var data = await File.ReadAllTextAsync(filePath);
                var objects = JsonSerializer.Deserialize<List<Medicine>>(data);

                if (objects is not null && objects.Any())
                {
                    context.Set<Medicine>().AddRange(objects);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Medicines seeded.");
                }
            }

            if (!context.Set<DeliveryPerson>().Any())
            {
                Console.WriteLine("Seeding Delivery Persons...");
                var filePath = Path.Combine(seedingPath, "deliveries.json");

                var data = await File.ReadAllTextAsync(filePath);
                var objects = JsonSerializer.Deserialize<List<DeliveryPerson>>(data);

                if (objects is not null && objects.Any())
                {
                    context.Set<DeliveryPerson>().AddRange(objects);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Delivery Persons seeded.");
                }
            }
            // Seed Nurses
            if (!context.Set<MedicalStaffMember>().Any(s => s.Role == MedicalRole.NURSE))
            {
                Console.WriteLine("Seeding Nurses...");
                var nurseFilePath = Path.Combine(seedingPath, "nurses.json");

                var nurseData = await File.ReadAllTextAsync(nurseFilePath);
                var nurses = JsonSerializer.Deserialize<List<MedicalStaffMember>>(nurseData);

                if (nurses is not null && nurses.Any())
                {
                    foreach (var nurse in nurses)
                    {
                        nurse.Role = MedicalRole.NURSE; // Ensure role is set correctly
                    }
                    context.Set<MedicalStaffMember>().AddRange(nurses);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Nurses seeded.");
                }
            }

            // Seed Doctors
            if (!context.Set<MedicalStaffMember>().Any(s => s.Role == MedicalRole.DOCTOR))
            {
                Console.WriteLine("Seeding Doctors...");
                var doctorFilePath = Path.Combine(seedingPath, "doctors.json");

                var doctorData = await File.ReadAllTextAsync(doctorFilePath);
                var doctors = JsonSerializer.Deserialize<List<MedicalStaffMember>>(doctorData);

                if (doctors is not null && doctors.Any())
                {
                    foreach (var doctor in doctors)
                    {
                        doctor.Role = MedicalRole.DOCTOR; // Ensure role is set correctly
                    }
                    context.Set<MedicalStaffMember>().AddRange(doctors);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Doctors seeded.");
                }
            }

            // Seed Assistants
            if (!context.Set<MedicalStaffMember>().Any(s => s.Role == MedicalRole.ASSIST))
            {
                Console.WriteLine("Seeding Assistants...");
                var assistantFilePath = Path.Combine(seedingPath, "assistants.json");

                var assistantData = await File.ReadAllTextAsync(assistantFilePath);
                var assistants = JsonSerializer.Deserialize<List<MedicalStaffMember>>(assistantData);

                if (assistants is not null && assistants.Any())
                {
                    foreach (var assistant in assistants)
                    {
                        assistant.Role = MedicalRole.ASSIST; // Ensure role is set correctly
                    }
                    context.Set<MedicalStaffMember>().AddRange(assistants);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Assistants seeded.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during DB seeding: {ex.Message}");
        }
    }
}

