using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Post;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.PostService.Command.Models
{
    public record UpdateHiringPostCommand(UpdateHiringPostDto dto,int createdBy):ICommand<ApiResponse>;
    public record UpdateExperiencePostCommand(UpdateExperiencePostDto dto,int createdBy):ICommand<ApiResponse>;
}
