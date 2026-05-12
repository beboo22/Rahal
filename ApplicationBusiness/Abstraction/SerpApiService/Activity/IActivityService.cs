// ============================================================
//  IActivityService
//  File: Services/IActivityService.cs
// ============================================================

using ApplicationBusiness.Dtos.Activity;
using Domain.BaseResponce;

namespace ApplicationBusiness.Abstraction.SerpApiService.Activity
{
    public interface IActivityService
    {
        /// <summary>
        /// Main entry point.
        /// For each day in the request it:
        ///   1. Resolves the first need → captures the GPS "Base Location".
        ///   2. Uses that Base Location's <c>ll</c> param to bias every
        ///      subsequent search on the same day (clustering effect).
        ///   3. Returns ≤3 options per need so the user can choose.
        /// </summary>
        Task<ApiResponse> CreateActivityByDayAsync(
            CreateActivityPlanRequest request,
            int userId,
            CancellationToken cancellationToken = default);
    }
}
