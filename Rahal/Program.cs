using Application.Fetures.Authentication.Command;
using Application.Fetures.Authentication.Command.Models;
using Application.Fetures.Authentication.Query;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using ApplicationBusiness.service;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Infrastructure;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Rahal.Middleware;
using System.Text;
namespace Rahal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var presentationLayerAssembly = typeof(Presentation.AssemblyReference).Assembly;

            // Add services to the container.
            builder.Services.AddControllers()
                .AddApplicationPart(presentationLayerAssembly)
                .AddControllersAsServices();
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(presentationLayerAssembly);
                cfg.RegisterServicesFromAssembly(typeof(ApplicationBusiness.AssemblyReference).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(Domain.AssemblyReference).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(Infrastructure.AssemblyReference).Assembly);

            });



            builder.Services.applayApplicationBusinessService(builder.Configuration);
            builder.Services.applayInfrastructureService(builder.Configuration);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"] ?? string.Empty))
                    };
                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Add JWT Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: \"Bearer eyJhbGciOi...\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                                                Array.Empty<string>()
                                            }
                                        });
            });


            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();  
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            using var scoped = app.Services.CreateScope();
            var service = scoped.ServiceProvider;
            var db = service.GetRequiredService<WriteSysDbContext>();
            try
            {
                if (!db.Set<Role>().Any())
                {
                    db.Roles.AddRange(new List<Role>()
                    {
                        new Role() { RoleName = "Admin"},
                        new Role() { RoleName = "Traveler"},
                        new Role() { RoleName = "TravelCompany"},
                        new Role() { RoleName = "TourGuide"},
                    });
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }





            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");
            app.Run();
        }
    }
}
