using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Auth;
using Domain.BaseResponce;

namespace ApplicationBusiness.Fetures.Authentication.Command.Models
{
    public record ResetRequestCommand(ResetRequestDto ResetRequestDto):ICommand<ApiResponse>;
}
