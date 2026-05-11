using Application.Abstraction.message;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.likesSerive.Command.Models
{
    public record AddLike(int postId,int UserId, LikeType LikeType):ICommand<ApiResponse>;
}
