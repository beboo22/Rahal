using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Post;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.PostService.Command.Models
{
    public record AddHiringPostCommand(AddHiringPostDto dto, int CreatedBy) :ICommand<ApiResponse>;
    public record AddExperiencePostCommand(AddExperiencePostDto dto, int CreatedBy):ICommand<ApiResponse>;


}
