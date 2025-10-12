using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PostService.Command.Models
{
    public record DeleteHiringPostCommand(int id, int createdBy) : ICommand<ApiResponse>;
    public record DeleteExperiencePostCommand(int id, int CreatedBy) : ICommand<ApiResponse>;
}
