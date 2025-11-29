using Application.Abstraction.message;
using Domain.BaseResponce;

namespace ApplicationBusiness.Fetures.Discount.Command.Models
{
    public record CreateGenericDiscountCommand(CreateGenericDiscountDto dto) : ICommand<ApiResponse>;

    public class CreateGenericDiscountDto
    {
        public int CreatorId { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }
    }
    public record UpdateGenericDiscountCommand(int Id, UpdateGenericDiscountDto dto) : ICommand<ApiResponse>;

    public class UpdateGenericDiscountDto
    {
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }
    }

    public record CreateSpecificDiscountCommand(CreateSpecificDiscountDto dto) : ICommand<ApiResponse>;

    public class CreateSpecificDiscountDto
    {

        public int TripId { get; set; }
        public int CreatorId { get; internal set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }
    }
    public record UpdateSpecificDiscountCommand(int Id, UpdateSpecificDiscountDto dto) : ICommand<ApiResponse>;

    public class UpdateSpecificDiscountDto
    {
        public int TripId { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }
    }

}