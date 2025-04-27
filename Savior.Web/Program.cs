
using Domain.Contracts;
using Domain.Models;
using Domain.Models.CartEntities;
using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Services;
using Services.MappingProfiles;
using ServicesAbstractions;

namespace Savior.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<SaviorDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString)
                       .LogTo(Console.WriteLine, LogLevel.Information)
                       .EnableSensitiveDataLogging();
            });


            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IGenericRepository<Pharmacy>, GenericRepository<Pharmacy>>();
            builder.Services.AddScoped<IGenericRepository<Medicine>, GenericRepository<Medicine>>();
            builder.Services.AddScoped<IGenericRepository<DeliveryPerson>, GenericRepository<DeliveryPerson>>();
            builder.Services.AddScoped<IGenericRepository<Cart>, GenericRepository<Cart>>();
            builder.Services.AddScoped<IGenericRepository<Order>, GenericRepository<Order>>();
            builder.Services.AddScoped<IGenericRepository<Emergency>, GenericRepository<Emergency>>();
            builder.Services.AddScoped<IGenericRepository<MedicalStaffMember>, GenericRepository<MedicalStaffMember>>();

            builder.Services.AddLogging();
            builder.Services.AddLogging(configure => configure.AddConsole());
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



            // Add services to the container
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IPharmacyService, PharmacyService>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IEmergencyService, EmergencyService>();
            builder.Services.AddScoped<IMedicalStaffMemberService, MedicalStaffMemberService>();

            builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
          options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
          options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
      });


            builder.Services.AddAutoMapper(typeof(PharmacyProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(MedicineProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(CartProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(OrderProfile).Assembly);


            builder.Services.AddLogging(configure => configure.AddConsole());



            builder.Services.AddScoped<IDbInitializer, DbInitializer>();


            builder.Services.AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();
                options.SchemaFilter<EnumSchemaFilter>(); 
 
                options.CustomSchemaIds(type => type.FullName);
            });




            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            await InitializerDbAsync(app);


            app.Run();
        }
        public static async Task InitializerDbAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await dbInitializer.InitializerAsync();
        }
    }
}
