using B2B_Procurement___Order_Management_Platform.Data;
using B2B_Procurement___Order_Management_Platform.Repositories.Identity;
using B2B_Procurement___Order_Management_Platform.Repositories.Products;
using B2B_Procurement___Order_Management_Platform.Services;
using B2B_Procurement___Order_Management_Platform.Services.Products;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace B2B_Procurement___Order_Management_Platform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            //// Data and Models
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");            
            builder.Services.AddDbContext<UserDb>(options =>
                options.UseNpgsql(connectionString)
            );
            builder.Services.AddDbContext<ProductDb>(options =>
               options.UseNpgsql(connectionString)
           );

            //// the Services
            builder.Services.AddScoped<IUserService, UserServices>();
            builder.Services.AddScoped<IProductServices, ProductServices>();

            /// the repository
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            //// the controller
            builder.Services.AddControllers();

            //// OpenAPI && Swagger
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();
        }
    }
}
