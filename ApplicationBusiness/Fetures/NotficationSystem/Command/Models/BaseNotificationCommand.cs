using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.NotficationSystem.Command.Models
{
    public record SendSystemNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public record SendFollowNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public record SendGuideRequestNotificationForPublicTripCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public record SendGuideRequestNotificationForPrivateTripCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public record SendMessageNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public record SendPaymentNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public record SendLikeNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public record SendCommentNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId); 
    public record SendBookingNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : BaseNotificationCommand(UserId, Title, Body, ReferenceId);
    public abstract record BaseNotificationCommand(
    string UserId,
    string Title,
    string Body,
    string ReferenceId
) : ICommand<ApiResponse>;
}
