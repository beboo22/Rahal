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
    public record CreateTravelerCompanyProfileCommand(CreateTravelerCompanyProfileDto dto, int Id):ICommand<ApiResponse>;
    public record CreateTourGuideProfileCommand(CreateTourGuideProfileDto dto, int Id):ICommand<ApiResponse>;
    public record CreateTravelerProfileCommand(CreateTravelerProfileDto dto, int Id):ICommand<ApiResponse>;
}
