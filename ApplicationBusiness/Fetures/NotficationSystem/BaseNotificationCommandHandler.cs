using Application.Abstraction.message;
using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using ApplicationBusiness.RealTimeservice.NotificationService;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.NotficationSystem
{
    public abstract class BaseNotificationCommandHandler<TCommand>
    : ICommandHandler<TCommand, ApiResponse>
    where TCommand : BaseNotificationCommand
    {
        private readonly INotificationService _notificationService;

        protected BaseNotificationCommandHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        protected abstract string NotificationType { get; }

        public async Task<ApiResponse> Handle(
            TCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var notification = new NotificationDto
                {
                    UserId = request.UserId,
                    Title = request.Title,
                    Body = request.Body,
                    Type = NotificationType,
                    ReferenceId = request.ReferenceId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _notificationService.SendAsync(notification);

                return new ApiResponse(
                    200,
                    "Notification sent successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    500,
                    $"Error while sending notification: {ex.Message}");
            }
        }
    }

    internal class SendBookingNotificationCommandHandler
    : BaseNotificationCommandHandler<SendBookingNotificationCommand>
    {
        public SendBookingNotificationCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.Booking;
    }

    internal class SendCommentNotificationCommandHandler
    : BaseNotificationCommandHandler<SendCommentNotificationCommand>
    {
        public SendCommentNotificationCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.Comment;
    }
    internal class SendLikeNotificationCommandHandler
    : BaseNotificationCommandHandler<SendLikeNotificationCommand>
    {
        public SendLikeNotificationCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.Like;
    }

    internal class SendPaymentNotificationCommandHandler
    : BaseNotificationCommandHandler<SendPaymentNotificationCommand>
    {
        public SendPaymentNotificationCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.Payment;
    }
    internal class SendMessageNotificationCommandHandler
    : BaseNotificationCommandHandler<SendMessageNotificationCommand>
    {
        public SendMessageNotificationCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.Message;
    }

    internal class SendGuideRequestNotificationForPublicTripCommandHandler
    : BaseNotificationCommandHandler<SendGuideRequestNotificationForPublicTripCommand>
    {
        public SendGuideRequestNotificationForPublicTripCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.GuideRequestForPublicTrip;
    }
    internal class SendGuideRequestNotificationForPrivateTripCommandHandler
    : BaseNotificationCommandHandler<SendGuideRequestNotificationForPrivateTripCommand>
    {
        public SendGuideRequestNotificationForPrivateTripCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.GuideRequestForPrivateTrip;
    }

    internal class SendSystemNotificationCommandHandler
    : BaseNotificationCommandHandler<SendSystemNotificationCommand>
    {
        public SendSystemNotificationCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.System;
    }
    internal class SendFollowNotificationCommandHandler
    : BaseNotificationCommandHandler<SendFollowNotificationCommand>
    {
        public SendFollowNotificationCommandHandler(
            INotificationService notificationService)
            : base(notificationService)
        {
        }

        protected override string NotificationType
            => NotificationTypes.Follow;
    }




}
