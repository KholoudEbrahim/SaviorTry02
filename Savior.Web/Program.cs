
using Domain.Contracts;
using Domain.Models;
using Domain.Models.CartEntities;
using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Repositories;
using Services;
using Services.MappingProfiles;
using ServicesAbstractions;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Savior.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();


            // DbContext
            builder.Services.AddDbContext<SaviorDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString)
                       .LogTo(Console.WriteLine, LogLevel.Information)
                       .EnableSensitiveDataLogging();
            });

            // Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IGenericRepository<Pharmacy>, GenericRepository<Pharmacy>>();
            builder.Services.AddScoped<IGenericRepository<Medicine>, GenericRepository<Medicine>>();
            builder.Services.AddScoped<IGenericRepository<DeliveryPerson>, GenericRepository<DeliveryPerson>>();
            builder.Services.AddScoped<IGenericRepository<Cart>, GenericRepository<Cart>>();
            builder.Services.AddScoped<IGenericRepository<Order>, GenericRepository<Order>>();
            builder.Services.AddScoped<IGenericRepository<Emergency>, GenericRepository<Emergency>>();
            builder.Services.AddScoped<IGenericRepository<MedicalStaffMember>, GenericRepository<MedicalStaffMember>>();

            // Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            builder.Services.AddTransient<EmailService>();
            builder.Services.AddTransient<NotificationService>();
            builder.Services.AddScoped<IPharmacyService, PharmacyService>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IEmergencyService, EmergencyService>();
            builder.Services.AddScoped<IMedicalStaffMemberService, MedicalStaffMemberService>();

            // JWT Authentication
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing!"));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IDbInitializer, DbInitializer>();


            // AutoMapper
            builder.Services.AddAutoMapper(typeof(PharmacyProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(MedicineProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(CartProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(OrderProfile).Assembly);

            // Logging
            builder.Services.AddLogging(configure => configure.AddConsole());

            // Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Savior API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer YOUR_TOKEN_HERE"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            });
       

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllers();
            await InitializerDbAsync(app);


            using (var scope = app.Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                if (!await unitOfWork.Pharmacies.AnyAsync(p => true)) 
                {
                    await unitOfWork.Pharmacies.AddAsync(new Pharmacy
                    {
                        Name = "Dr.Noha Pharmacy",
                        City = "Cairo",
                        Street = "BourSaid Street",
                        BuildingNumber = "1",
                        HasDelivery = true,
                        Latitude = 30.0444,
                        Longitude = 31.2357
                    });
                    await unitOfWork.CompleteAsync();
                }
            }


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