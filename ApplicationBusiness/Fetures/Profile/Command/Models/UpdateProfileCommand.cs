using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Profile;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.Profile.Command.Models
{
    public record UpdateTravelerCompanyProfileCommand(UpdateTravelerCompanyProfileDto dto,int Id):ICommand<ApiResponse>;
    public record UpdateTourGuideProfileCommand(UpdateTourGuideProfileDto dto,int Id):ICommand<ApiResponse>;
    public record UpdateTravelerProfileCommand(UpdateTravelerProfileDto dto, int Id):ICommand<ApiResponse>;
}
