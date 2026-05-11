using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Status;
using Domain.BaseResponce;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.StatusService.Command.Model
{
    public record AddStatus(AddStatusDto req, int CreatedById) :ICommand<ApiResponse>;
    public record DeleteStatus(int StatusId,int CreatedById):ICommand<ApiResponse>;
    public record ViewStatus(int StatusId,int ViewerId):ICommand<ApiResponse>;
    public record LoveStatus(int StatusId,int ViewerId,bool love):ICommand<ApiResponse>;
    public record IsStatusExist(int StatusId):ICommand<ApiResponse>;

    
}
