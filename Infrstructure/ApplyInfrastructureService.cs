using Domain.Abstraction;
using Infrastructure.Data;
using InfraStructure.Impelementation;
using Infrstructure.Impelementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ApplyInfrastructureService
    {

        public static void applayInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            //how call it in program.cs
            //services.applayInfrastructureService(configuration);

            //connect to the database
            services.AddDbContext<ReadSysDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), Soptions => Soptions.CommandTimeout(60))
                // Set command timeout to 60 seconds
                );
            services.AddDbContext<WriteSysDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), Soption => Soption.CommandTimeout(60)));

            // Register other infrastructure services here
            // e.g., services.AddScoped<IRepository, Repository>();
            services.AddScoped<IReadUnitOfWork, ReadUnitOfWork>();
            services.AddScoped<IWriteUnitOfWork, WriteUnitOfWork>();
            services.AddScoped(typeof(IWriteGenericRepo<>), typeof(WriteGenericRepo<>));
            services.AddScoped(typeof(IReadGenericRepo<>), typeof(ReadGenericRepo<>));
            services.AddScoped(typeof(IReadUserRepo<>), typeof(ReadUserRepo<>));
            services.AddScoped(typeof(IWriteUserRepo), typeof(WriteUserRepo));
            services.AddScoped(typeof(IGoogleDriveRepo), typeof(GoogleDrvieRepository));
            services.AddScoped(typeof(IPhotoService), typeof(PhotoService));


            services.AddScoped<IReadHotelSearchHistoryRepository, ReadHotelSearchHistoryRepository>();
            services.AddScoped<IWriteHotelSearchHistoryRepository, WriteHotelSearchHistoryRepository>();

            services.AddScoped<IReadFlightSearchHistoryRepository, ReadFlightSearchHistoryRepository>();
            services.AddScoped<IWriteFlightSearchHistoryRepository, WriteFlightSearchHistoryRepository>();



        }
    }
}
