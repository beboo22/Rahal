using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TripEntity;
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
            _logger.LogInformation("Background cleanup service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var db = scope.ServiceProvider
                        .GetRequiredService<WriteSysDbContext>();

                    await DeleteExpiredStories(db, stoppingToken);

                    await UnblockExpiredUsers(db, stoppingToken);

                    await FinishExpiredTrips(db, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing cleanup jobs.");
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }

            _logger.LogInformation("Background cleanup service stopped.");
        }

        private async Task DeleteExpiredStories(
            WriteSysDbContext db,
            CancellationToken stoppingToken)
        {
            var deletedStories = await db.Status
                .Where(s => s.CreatedAt.AddHours(24) < DateTime.UtcNow)
                .ExecuteDeleteAsync(stoppingToken);

            if (deletedStories > 0)
            {
                _logger.LogInformation(
                    "{Count} expired stories deleted.",
                    deletedStories);
            }
        }

        private async Task UnblockExpiredUsers(
            WriteSysDbContext db,
            CancellationToken stoppingToken)
        {
            var updatedUsers = await db.Users
                .Where(u =>
                    u.IsBlocked == true &&
                    u.BlockedEndDate.HasValue &&
                    u.BlockedEndDate <= DateTime.UtcNow)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.IsBlocked, false)
                    .SetProperty(u => u.BlockedEndDate, (DateTime?)null)
                    .SetProperty(u => u.BlockedStartDate, (DateTime?)null),
                    stoppingToken);

            if (updatedUsers > 0)
            {
                _logger.LogInformation(
                    "{Count} users unblocked.",
                    updatedUsers);
            }
        }

        private async Task FinishExpiredTrips(
            WriteSysDbContext db,
            CancellationToken stoppingToken)
        {
            var finishedPublicTrips = await db.PublicTrips
                .Where(t =>
                    t.TripStatus != TripStatus.Finished &&
                    t.StartDate.HasValue &&
                    t.StartDate.Value.AddDays(t.Duration) < DateTime.UtcNow)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.TripStatus, TripStatus.Finished),
                    stoppingToken);

            var finishedPrivateTrips = await db.PrivateTrips
                .Where(t =>
                    t.TripStatus != TripStatus.Finished &&
                    t.StartDate.HasValue &&
                    t.StartDate.Value.AddDays(t.Duration) < DateTime.UtcNow)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.TripStatus, TripStatus.Finished),
                    stoppingToken);

            if (finishedPublicTrips > 0 || finishedPrivateTrips > 0)
            {
                _logger.LogInformation(
                    "{PublicCount} public trips and {PrivateCount} private trips marked as finished.",
                    finishedPublicTrips,
                    finishedPrivateTrips);
            }
        }
    }
}
