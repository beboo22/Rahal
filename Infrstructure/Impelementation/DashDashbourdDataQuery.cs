using Domain.Abstraction;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Impelementation
{
    internal class DashDashbourdDataQuery : IDashDashbourdDataQuery
    {
        ReadSysDbContext ReadSysDbContext;

        public DashDashbourdDataQuery(ReadSysDbContext readSysDbContext)
        {
            ReadSysDbContext = readSysDbContext;
        }

        public async Task<GetDashbourdDataDto> GetDashbourdDataDtoAsync()
        {
            var totalUsers = await ReadSysDbContext.Users
                .AsNoTracking()
                .CountAsync();

            var companyStats = await ReadSysDbContext.TravelCompanies
                .AsNoTracking()
                .GroupBy(x => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Unverified = g.Count(x => !x.User.Isverified)
                })
                .FirstOrDefaultAsync();

            var guideStats = await ReadSysDbContext.TourGuides
                .AsNoTracking()
                .GroupBy(x => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Unverified = g.Count(x => !x.User.Isverified)
                })
                .FirstOrDefaultAsync();

            var postStats = await ReadSysDbContext.ExperiencePosts
                .AsNoTracking()
                .GroupBy(x => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Invalid = g.Count(x => !x.IsValid)
                })
                .FirstOrDefaultAsync();

            var monthlyUsers = await ReadSysDbContext.Users
                .AsNoTracking()
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new MonthlyUserCreation
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    UserCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            var invalidPostsMonthly = await ReadSysDbContext.ExperiencePosts
                .AsNoTracking()
                .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                .Select(g => new PercentageUvalidPostEverymonth
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    PercentageUnvalidPosts =
                        g.Count() == 0
                            ? 0
                            : (double)g.Count(x => !x.IsValid) / g.Count() * 100
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            var totalTrips = await ReadSysDbContext.PublicTrips
                .AsNoTracking()
                .CountAsync();

            var topDestinations = await ReadSysDbContext.PublicTrips
                .AsNoTracking()
                .GroupBy(t => t.Destination)
                .Select(g => new
                {
                    Destination = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            return new GetDashbourdDataDto
            {
                TotalUsers = totalUsers,

                TotalTravelcompany = companyStats?.Total ?? 0,
                TotalUnverifiedTravelcompany = companyStats?.Unverified ?? 0,

                TotalTourgiude = guideStats?.Total ?? 0,
                TotalUnverifiedTourgiude = guideStats?.Unverified ?? 0,

                TotalPost = postStats?.Total ?? 0,
                TotalUnValidPost = postStats?.Invalid ?? 0,

                PercentageUvalidPost =
                    postStats == null || postStats.Total == 0
                        ? 0
                        : (double)postStats.Invalid / postStats.Total * 100,

                ToTalUserCreatedInEveryMonth = monthlyUsers,

                PercentageUvalidPostEverymonth = invalidPostsMonthly,

                TopDestinationInTrips = topDestinations
                    .Select(x => new TopDestinationInTrips
                    {
                        Destination = x.Destination,
                        Count = x.Count,
                        PercentageDestination =
                            totalTrips == 0
                                ? 0
                                : (double)x.Count / totalTrips * 100
                    })
                    .ToList()
            };
        }
    }

    
}
