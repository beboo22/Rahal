using Application.Abestraction;
using Application.Abstraction.spacification;
using Application.Abstraction.Specification;
using Application.Features.PaymentService;
using ApplicationBusiness.Abstraction.CloudinaryService;
using ApplicationBusiness.Fetures.PaymentService.Strategies;
using ApplicationBusiness.RealTimeservice.ChatService;
using ApplicationBusiness.service;
using Infrastructure.Abestraction;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Abestraction;
using System.Security.Claims;

namespace ApplicationBusiness
{
    public static class ApplyApplicationBusinessService
    {

        public static void applayApplicationBusinessService(this IServiceCollection services, IConfiguration configuration)
        {
            //how call it in program.cs
            //services.applayInfrastructureService(configuration);

            //connect to the database

            //services.AddScoped(typeof(IAuthentication),typeof(Authentication<>));
            services.AddScoped(typeof(IAuthentication), typeof(Authentication));
            services.AddScoped(typeof(IPaymobService), typeof(PaymobService));
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddScoped(typeof(ISpecification<>), typeof(Specification<>));
            services.AddScoped<IPaymentHandlerStrategy, PublicTripPaymentHandler>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IPaymentHandlerStrategy, PrivateTripPaymentHandler>();
            //services.AddScoped<IPaymentHandlerStrategy, HotelPaymentHandler>();
            //services.AddScoped<IPaymentHandlerStrategy, FlightPaymentHandler>();

            services.AddScoped<PaymentHandlerFactory>();
            services.AddSignalR(options =>
            {
                // optional configuration
            }).AddHubOptions<ChatHub>(options =>
            {
                // nothing special needed here yet
            });
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

        }
    }




    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Try to extract from JWT claim "nameidentifier" (default)
            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // OR, fallback to query string for debugging
            if (string.IsNullOrEmpty(userId))
                userId = connection.GetHttpContext()?.Request.Query["userId"];

            return userId;
        }
    }

}
