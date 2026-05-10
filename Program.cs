using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.Services;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Domain.Models;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Infrastructure.Repositories;
using B2B_Procurement___Order_Management_Platform.src.ArtMarket.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDb>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            builder.Services.AddAuthentication(options =>         /// to map JWT section in appsettings.json --> in class JWT.cs 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))

                    };

                });
                var jwtSection = builder.Configuration.GetSection("JWT");
                Console.WriteLine($"Key: {jwtSection["Key"]}");
                Console.WriteLine($"Issuer: {jwtSection["Issuer"]}");


            ////// the Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            

            ///// the repository
            builder.Services.AddScoped<IAuthRepo, AuthRepo>();

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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
