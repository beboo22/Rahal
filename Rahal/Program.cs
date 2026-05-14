using ApplicationBusiness;
using ApplicationBusiness.service;
using Domain.Entity.Identity;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Rahal.Middleware;
using System.Text;
using Domain.Abstraction;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Configuration;
using Polly;
using StackExchange.Redis;
using ApplicationBusiness.Abstraction.SerpApiService.Activity;
using Domain.BaseResponce;
using ApplicationBusiness.RealTimeservice;
using Scalar.AspNetCore;

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
            // ...
            // هيقرأ الـ Section اللي عملناها في appsettings.json
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            // ...



            builder.Services.applayApplicationBusinessService(builder.Configuration);
            builder.Services.applayInfrastructureService(builder.Configuration);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                            // اسحب التوكن من الـ Query String
                            var accessToken = context.Request.Query["access_token"];

                            // تأكد إن الطلب رايح للـ Hub بتاعك
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/realtimehub")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },

                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";

                            var response = new ApiResponse(403, "Access Denied: You do not have the required permissions/role to access this resource.");

                            var json = System.Text.Json.JsonSerializer.Serialize(response);
                            await context.Response.WriteAsync(json);
                        },

                        // 2. 401 Unauthorized: Issues with the token itself or missing token
                        OnChallenge = async context =>
                        {
                            // Skip the default logic to provide our custom response
                            context.HandleResponse();

                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = 401;
                                context.Response.ContentType = "application/json";

                                // Default message: No token provided
                                string message = "Unauthorized: Authentication token is missing.";

                                // Case: Token is expired
                                if (context.AuthenticateFailure != null && context.AuthenticateFailure.Message.Contains("expired"))
                                {
                                    message = "Unauthorized: Your session has expired. Please refresh your token or login again.";
                                }
                                // Case: Token is present but invalid/tampered with
                                else if (context.Error == "invalid_token")
                                {
                                    message = "Unauthorized: The provided token is invalid or malformed.";
                                }

                                var response = new ApiResponse(401, message);
                                var json = System.Text.Json.JsonSerializer.Serialize(response);
                                await context.Response.WriteAsync(json);
                            }
                        },

                        // 3. Log technical failures for debugging
                        OnAuthenticationFailed = async context =>
                        {
                            // Adding a header for the frontend dev to see the technical reason in the Network tab
                            context.Response.Headers.Append("X-Token-Error", context.Exception.Message);
                            await Task.CompletedTask;
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


            //builder.Services.AddHttpClient<IAmadeusAuthService, AmadeusAuthService>();
            //builder.Services.AddHttpClient<IHotelApiClient, HotelApiClient>();
            //builder.Services.AddScoped<IHotelService, HotelService>();
            //builder.Services.AddScoped<IFlightAvailabilityService, FlightAvailabilityService>();


            builder.Services.Configure<SerpApiSettings>(
             builder.Configuration.GetSection(SerpApiSettings.SectionName));

            // ── Validators ────────────────────────────────────────────────────────
            //builder.Services.AddFluentValidationAutoValidation();
            //builder.Services.AddScoped<IValidator<FlightSearchRequest>, FlightSearchRequestValidator>();
            //builder.Services.AddScoped<IValidator<HotelSearchRequest>, HotelSearchRequestValidator>();

            // ── Polly Policies ────────────────────────────────────────────────────
            //var retryPolicy = HttpPolicyExtensions
            //    .HandleTransientHttpError()
            //    .WaitAndRetryAsync(
            //        retryCount: 3,
            //        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt)),
            //        onRetry: (outcome, timespan, retryAttempt, _) =>
            //        {
            //            Console.WriteLine($"[SerpAPI] Retry {retryAttempt} after {timespan.TotalMs}ms — {outcome.Exception?.Message}");
            //        });

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                builder.Configuration.GetValue("SerpApi:TimeoutSeconds", 30));

            // ── HttpClient + Polly ────────────────────────────────────────────────
            builder.Services.AddHttpClient<ISerpApiService, SerpApiService>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "TravelModule/1.0");
            });
            // ── HttpClient + Polly ────────────────────────────────────────────────
            builder.Services.AddHttpClient<ISerpPhotoApiService, SerpPhotoApiService>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "TravelModule/1.0");
            });
            builder.Services.AddHttpClient<IActivityService, ActivityService>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "TravelModule/1.0");
            });
            //.AddPolicyHandler(retryPolicy)
            //.AddPolicyHandler(timeoutPolicy);

            // ── Repositories ──────────────────────────────────────────────────────
            // Replace these with your EF Core implementations:
            // builder.Services.AddScoped<IFlightSearchHistoryRepository, FlightSearchHistoryRepository>();
            // builder.Services.AddScoped<IHotelSearchHistoryRepository, HotelSearchHistoryRepository>();

            // ── Redis Caching (optional) ──────────────────────────────────────────
            var redisConn = builder.Configuration.GetRequiredSection("Redis")["ConnectionString"];// .GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConn))
            {
                //builder.Services.AddStackExchangeRedisCache(options =>
                //{
                //    options.Configuration = redisConn;
                //    options.InstanceName = "Rahal:";
                //});
                builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                                ConnectionMultiplexer.Connect(redisConn));
            }
            else
            {
                builder.Services.AddDistributedMemoryCache(); // fallback to in-memory
            }



            //builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            //{
            //    var configuration = new ConfigurationOptions
            //    {
            //        EndPoints =
            //        {
            //            { "redis-19301.c341.af-south-1-1.ec2.redns.redis-cloud.com", 19301 }
            //        },
            //        User = "default",
            //        Password = "uJhzvCJD1pjVz9lBh4gKVc9OrKRL9pTR",
            //        Ssl = true,
            //        AbortOnConnectFail = false
            //    };

            //    return ConnectionMultiplexer.Connect(configuration);
            //});



            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            // Explicitly point Scalar to the Swagger JSON endpoint
            app.MapScalarApiReference("", options =>
            {
                options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
            });
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            using var scoped = app.Services.CreateScope();
            var service = scoped.ServiceProvider;
            var db = service.GetRequiredService<WriteSysDbContext>();
            try
            {
                if (!db.Set<Domain.Entity.Identity.Role>().Any())
                {
                    db.Roles.AddRange(new List<Domain.Entity.Identity.Role>()
                    {
                        new Domain.Entity.Identity.Role() { RoleName = RoleEnum.Admin},
                        new Domain.Entity.Identity.Role() { RoleName = RoleEnum.Traveler},
                        new Domain.Entity.Identity.Role() { RoleName = RoleEnum.TravelCompany},
                        new Domain.Entity.Identity.Role() { RoleName = RoleEnum.TourGuide},
                    });
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }





            app.MapControllers();
            app.MapHub<AppHub>("/realtimehub");
            app.Run();
        }
    }
}
