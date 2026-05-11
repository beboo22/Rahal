using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{

    public class StoryCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StoryCleanupService> _logger;

        public StoryCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<StoryCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Story cleanup service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var dbContext = scope.ServiceProvider
                        .GetRequiredService<WriteSysDbContext>();

                    var deletedCount = await dbContext.Status
                        .Where(x => x.EndDate <= DateTime.UtcNow)
                        .ExecuteDeleteAsync(stoppingToken);
                    if (deletedCount > 0)
                    {
                        //await dbContext.SaveChangesAsync();
                        _logger.LogInformation(
                            "{Count} expired stories deleted at {Time}",
                            deletedCount,
                            DateTime.UtcNow);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while deleting expired stories.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Story cleanup service stopped.");
        }
    }
}
