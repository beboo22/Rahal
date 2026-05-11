using Domain.BaseResponce;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Follow.Query.Models
{
    public record IsFollowingQuery(int FollowerId, int FollowingId) : IRequest<ApiResponse>;
}
