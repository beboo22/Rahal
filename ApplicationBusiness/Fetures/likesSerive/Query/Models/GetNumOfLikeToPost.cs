using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.likesSerive.Query.Models
{
    public record GetUserLikeToPost(int postId):IQuery<ApiResponse>;
}
