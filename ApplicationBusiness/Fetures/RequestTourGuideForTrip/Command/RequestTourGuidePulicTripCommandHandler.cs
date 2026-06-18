using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using ApplicationBusiness.Fetures.TripService.Command;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.RequestTourGuideForTrip.Command
{
    public record RequestTourGuidePubTripCommand(int userId, int Trip, List<int> TourguideIds) : ICommand<ApiResponse>;
    public record AcceptPubRequest(int RequestId) : ICommand<ApiResponse>;


    internal class RequestTourGuidePulicTripCommandHandler : ICommandHandler<RequestTourGuidePubTripCommand, ApiResponse>,
        ICommandHandler<AcceptPubRequest, ApiResponse>
    {
        private IWriteGenericRepo<RequestTourGuidePulicTrip> WriteGenericRepo { get; set; }
        private IReadGenericRepo<RequestTourGuidePulicTrip> ReadGenericRepo { get; set; }

        private IWriteUnitOfWork writeUnitOfWork { get; set; }
        public ISender Sender { get; set; }
        public RequestTourGuidePulicTripCommandHandler(IWriteGenericRepo<RequestTourGuidePulicTrip> writeGenericRepo, IWriteUnitOfWork writeUnitOfWork, ISender sender, IReadGenericRepo<RequestTourGuidePulicTrip> readGenericRepo)
        {
            WriteGenericRepo = writeGenericRepo;
            this.writeUnitOfWork = writeUnitOfWork;
            Sender = sender;
            ReadGenericRepo = readGenericRepo;
        }


        public async Task<ApiResponse> Handle(RequestTourGuidePubTripCommand request, CancellationToken cancellationToken)
        {
            // 1. التأكد من وجود المرشدين أولاً
            foreach (var TourguideId in request.TourguideIds)
            {
                var cheacktour = await Sender.Send(new CheckTourguideExsist(TourguideId));
                if (cheacktour.statusCode != StatusCodes.Status302Found)
                    return new ApiResponse(StatusCodes.Status404NotFound, $"Can't find tourguide with id {TourguideId}");
            }

            // 2. جلب بيانات الرحلة والتحقق من الشروط
            var trip = await Sender.Send(new GetPubTripSpecQuery(new TripFilter
            {
                Id = request.Trip,
            })) as ApiResultResponse<TemplateTrip>;

            if (trip?.Data == null)
                return new ApiResponse(404);

            if (request.userId != trip.Data.CreatedById)
                return new ApiResponse(StatusCodes.Status403Forbidden, "You are not the owner of this trip, so you can't request tourguide for it");

            if (trip.Data.TripStatus == TripStatus.Published)
                return new ApiResponse(StatusCodes.Status400BadRequest, "Can't request tourguide for this trip, bec it published already");

            // ------------------------------------------------------------
            // التعديل الجديد: الفلترة لمنع التكرار لنفس الـ Trip ونفس الـ Guide
            // ------------------------------------------------------------
            var validTourguideIds = new List<int>();

            foreach (var TourguideId in request.TourguideIds)
            {
                // افترضنا هنا وجود ميثود في الـ ReadGenericRepo بتجيب بالـ Specification أو الـ Expression
                // لو مش متوفرة، تقدر تعدلها حسب الـ Repository Pattern اللي شغال بيه
                var isAlreadyRequested = await ReadGenericRepo.GetAll().AnyAsync(x => x.PublicTripId == request.Trip && x.TourGuideId == TourguideId);

                if (!isAlreadyRequested)
                {
                    validTourguideIds.Add(TourguideId);
                }
            }

            // إذا كانت كل المعرفات المرسلة مبعوت لها طلبات مسبقاً
            if (!validTourguideIds.Any())
            {
                return new ApiResponse(StatusCodes.Status400BadRequest, "All selected tour guides have already been requested for this trip.");
            }
            // ------------------------------------------------------------

            var item = new List<RequestTourGuidePulicTrip>();

            // نستخدم القائمة المفلترة فقط (validTourguideIds) بدلاً من القائمة القديمة
            foreach (var TourguideId in validTourguideIds)
            {
                item.Add(new RequestTourGuidePulicTrip
                {
                    PublicTripId = request.Trip,
                    TourGuideId = TourguideId,
                });
            }

            var res = await Sender.Send(new UpdatePubTripStatus(trip.Data.CreatedById, trip.Data.Id, TripStatus.WaitingForGuideApproval));
            if (res.statusCode != 200)
                return res;

            try
            {
                await writeUnitOfWork.BeginTransactionAsync();
                await WriteGenericRepo.AddRangAsync(item);
                await writeUnitOfWork.SaveChangesAsync();
                await writeUnitOfWork.CommitAsync();

                // إرسال الإشعارات فقط للمرشدين الجدد الذين تم حفظهم بالفعل
                foreach (var TourguideId in validTourguideIds)
                {
                    await Sender.Send(
                    new SendGuideRequestNotificationForPublicTripCommand(
                        TourguideId.ToString(),
                        "New Guide Request to Public Trip 🧭",
                        $"{trip.Data.Title} requested you to a guide.",
                        trip.Data.Id.ToString()
                    ));
                }
                return new ApiResponse(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                await writeUnitOfWork.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }
        }


        public async Task<ApiResponse> Handle(AcceptPubRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var item = await ReadGenericRepo.GetByIdAsync(request.RequestId);

                if (item == null)
                    return new ApiResponse(404);

                if (item.Accept == true)
                    return new ApiResponse(StatusCodes.Status204NoContent, "some one acc");


                bool isAlreadyAcceptedByAnother = await ReadGenericRepo.GetAll()
                    .AnyAsync(x => x.PublicTripId == item.PublicTripId && x.Accept == true, cancellationToken);


                if (isAlreadyAcceptedByAnother)
                {
                    return new ApiResponse(
                    StatusCodes.Status400BadRequest,
                    "Sorry, this trip has already been accepted by another tour guide."
                    );
                }


                var trip = await Sender.Send(new GetPubTripSpecQuery(new TripFilter
                {
                    Id = item.PublicTripId,
                })) as ApiResultResponse<TemplateTrip>;
                if (trip?.Data == null)
                    return new ApiResponse(404);


                var res = await Sender.Send(new UpdatePubTripStatus(trip.Data.CreatedById, item.PublicTripId, TripStatus.GuideAssigned));
                if (res.statusCode != 200)
                    return res;


                item.Accept = true;
                item.AcceptedAt = DateTime.UtcNow;
                var updatetrip = await Sender.Send(new AddTourguideToPubTrip(item.TourGuideId, item.PublicTripId));
                if (updatetrip.statusCode != 200)
                    return updatetrip;
                await writeUnitOfWork.BeginTransactionAsync();
                await WriteGenericRepo.UpdateAsync(item, item.Id);
                await writeUnitOfWork.SaveChangesAsync();
                await writeUnitOfWork.CommitAsync();



                return new ApiResponse(200);
            }
            catch (Exception ex)
            {
                await writeUnitOfWork.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }


        }

    }

}
