using Application.Abstraction.Specification;
using Domain.Entity.Hotel_flights;
using Domain.Entity.Identity;
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
        Task<bool> BlockUserrAsync(int id,DateTime from,DateTime to);
    }

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
