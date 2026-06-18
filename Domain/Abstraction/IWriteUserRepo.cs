using Application.Abstraction.Specification;
using Domain.Entity.Hotel_flights;
using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using Domain.Entity.Status;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstraction
{
    public interface IWriteUserRepo: IWriteGenericRepo<User>
    {
        Task<bool> ExistsAsync(string Email);
        Task<bool> ExistsAsync(int UserId);
        Task<bool> BlockUserrAsync(int id,DateTime from,DateTime to);
    }
    public interface IDashDashbourdDataQuery
    {
        Task<GetDashbourdDataDto> GetDashbourdDataDtoAsync();
    }

    public interface IWriteStatusUserRepo : IWriteGenericRepo<StatusUser>
    {
        Task<bool> ExistsAsync(int UserId,int StatusId);
    }
    public interface IWriteExPostRepo : IWriteGenericRepo<ExperiencePost>
    {
    }
    public interface IWritepubTripRepo : IWriteGenericRepo<PublicTrip>
    {
    }
    //public interface IWriteHirPostRepo : IWriteGenericRepo<HiringPost>
    //{
    //}

    public interface IWriteFlightSearchHistoryRepository: IWriteGenericRepo<FlightSearchHistory> 
    {
       

        Task AddAsync(FlightSearchHistory entity, CancellationToken cancellationToken = default);
    }
    public interface IReadFlightSearchHistoryRepository:IReadGenericRepo<FlightSearchHistory>
    {

        Task<IReadOnlyList<FlightSearchHistory>> ListAsync(
           ISpecification<FlightSearchHistory> spec,
           CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            ISpecification<FlightSearchHistory> spec,
            CancellationToken cancellationToken = default);
    }
    public interface IReadHotelSearchHistoryRepository:IReadGenericRepo<HotelSearchHistory>
    {
        Task<IReadOnlyList<HotelSearchHistory>> ListAsync(
            ISpecification<HotelSearchHistory> spec,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            ISpecification<HotelSearchHistory> spec,
            CancellationToken cancellationToken = default);
    } public interface IWriteHotelSearchHistoryRepository:IWriteGenericRepo<HotelSearchHistory>
    {
        Task AddAsync(HotelSearchHistory entity, CancellationToken cancellationToken = default);
    }


}
