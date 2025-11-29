using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Discount.Query.Models
{
    public record GetAllGenericDiscountsQuery() : IQuery<ApiResponse>;
    public record GetGenericDiscountByCodeQuery(string Code) : IQuery<ApiResponse>;
    public record GetAllSpecificDiscountsQuery(int CreatorId) : IQuery<ApiResponse>;
    public record GetSpecificDiscountByCodeQuery(string Code) : IQuery<ApiResponse>;

}
