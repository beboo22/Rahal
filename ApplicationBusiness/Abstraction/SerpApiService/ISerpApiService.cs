using Application.Abstraction.Specification;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.SerpApiService
{
    public interface ISerpApiService
    {
        Task<ApiResponse> SearchFlightsAsync(
            FlightSearchRequest request,
            CancellationToken cancellationToken = default);

        Task<ApiResponse> SearchHotelsAsync(
            HotelSearchRequest request,
            CancellationToken cancellationToken = default);
    }

    public interface IFlightSearchHistoryRepository
    {
        Task<IReadOnlyList<FlightSearchHistory>> ListAsync(
            ISpecification<FlightSearchHistory> spec,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            ISpecification<FlightSearchHistory> spec,
            CancellationToken cancellationToken = default);

        Task AddAsync(FlightSearchHistory entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
    public interface IHotelSearchHistoryRepository
    {
        Task<IReadOnlyList<HotelSearchHistory>> ListAsync(
            ISpecification<HotelSearchHistory> spec,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            ISpecification<HotelSearchHistory> spec,
            CancellationToken cancellationToken = default);

        Task AddAsync(HotelSearchHistory entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
