using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Data;
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
            builder.Services.AddDbContext<AppDb>(options =>
                options.UseNpgsql(connectionString)
            );


            ////// the Services
            //builder.Services.AddScoped<IUserService, UserServices>();
            builder.Services.AddHealthChecks();
            

            ///// the repository
            //builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddScoped<IProductRepository, ProductRepository>();

            ////// the controller
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

            //app.UseAuthorization();

            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
