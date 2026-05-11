using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Follow.Command.Models
{
    public record FollowComand(int Follower, int person):ICommand<ApiResponse>;
    public record UnfollowCommand(int Follower, int person):ICommand<ApiResponse>;
}
