using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Post;
using Domain.BaseResponce;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.CommentService.Command.Model
{
    public record AddcommentToHiringPost(int postId,CommnetDto Comment):ICommand<ApiResponse>;
    public record AddcommentToExperiencePost(int postId,CommnetDto Comment):ICommand<ApiResponse>;
}
