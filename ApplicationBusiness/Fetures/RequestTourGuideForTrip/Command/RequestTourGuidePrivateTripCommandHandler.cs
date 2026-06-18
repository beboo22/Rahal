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


    public record RequestTourGuidePrivateTripCommand(int userId,int Trip, List<int> TourguideIds) : ICommand<ApiResponse>;
    public record AcceptPivRequest(int RequestId) : ICommand<ApiResponse>;

    internal class RequestTourGuidePrivateTripCommandHandler : ICommandHandler<RequestTourGuidePrivateTripCommand, ApiResponse>,
        ICommandHandler<AcceptPivRequest, ApiResponse>
    {
        private IWriteGenericRepo<RequestTourGuidePrivateTrip> WriteGenericRepo { get; set; }
        private IReadGenericRepo<RequestTourGuidePrivateTrip> ReadGenericRepo { get; set; }

        private IWriteUnitOfWork writeUnitOfWork { get; set; }
        public ISender Sender { get; set; }

        public RequestTourGuidePrivateTripCommandHandler(ISender sender, IWriteUnitOfWork writeUnitOfWork, IReadGenericRepo<RequestTourGuidePrivateTrip> readGenericRepo, IWriteGenericRepo<RequestTourGuidePrivateTrip> writeGenericRepo)
        {
            Sender = sender;
            this.writeUnitOfWork = writeUnitOfWork;
            ReadGenericRepo = readGenericRepo;
            WriteGenericRepo = writeGenericRepo;
        }

        public async Task<ApiResponse> Handle(RequestTourGuidePrivateTripCommand request, CancellationToken cancellationToken)
        {
            foreach (var TourguideId in request.TourguideIds)
            {
                var cheacktour = await Sender.Send(new CheckTourguideExsist(TourguideId));
                if (cheacktour.statusCode != StatusCodes.Status302Found)
                    return new ApiResponse(StatusCodes.Status404NotFound, "Can't found tourguide");
            }

            var trip = await Sender.Send(new GetPrivTripSpecQuery(new TripFilter
            {
                Id = request.Trip,
            })) as ApiResultResponse<PrivateTemplateTrip>;

            if (trip?.Data == null)
                return new ApiResponse(404);

            if (request.userId != trip.Data.CreatedById)
                return new ApiResponse(StatusCodes.Status403Forbidden, "You are not the owner of this trip, so you can't request tourguide for it");

            if (trip.Data.TripStatus == TripStatus.Published)
                return new ApiResponse(StatusCodes.Status400BadRequest, "Can't request tourguide for this trip, bec it published already");

            // ------------------------------------------------------------
            // منع التكرار: فلترة الـ IDs لمنع إرسال طلب جديد إذا كان هناك طلب قائم بالفعل لنفس الـ Trip والـ Guide
            // ------------------------------------------------------------
            var validTourguideIds = new List<int>();

            foreach (var TourguideId in request.TourguideIds)
            {
                // استخدام الـ ReadGenericRepo الممرر في الـ Constructor للتحقق من عدم وجود ريكورد مسبق
                var isAlreadyRequested = await ReadGenericRepo.GetAll().AnyAsync(x => x.PrivateTripId == request.Trip && x.TourGuideId == TourguideId);

                if (!isAlreadyRequested)
                {
                    validTourguideIds.Add(TourguideId);
                }
            }

            if (!validTourguideIds.Any())
            {
                return new ApiResponse(StatusCodes.Status400BadRequest, "All selected tour guides have already been requested for this trip.");
            }
            // ------------------------------------------------------------

            var item = new List<RequestTourGuidePrivateTrip>();

            foreach (var TourguideId in validTourguideIds)
            {
                item.Add(new RequestTourGuidePrivateTrip
                {
                    PrivateTripId = request.Trip,
                    TourGuideId = TourguideId,
                });
            }

            var res = await Sender.Send(new UpdatePrivTripStatus(trip.Data.CreatedById, trip.Data.Id, TripStatus.WaitingForGuideApproval));
            if (res.statusCode != 200)
                return res;

            try
            {
                await writeUnitOfWork.BeginTransactionAsync();
                await WriteGenericRepo.AddRangAsync(item);
                await writeUnitOfWork.SaveChangesAsync();
                await writeUnitOfWork.CommitAsync();

                foreach (var TourguideId in validTourguideIds)
                {
                    await Sender.Send(
                    new SendGuideRequestNotificationForPublicTripCommand(
                        TourguideId.ToString(),
                        "New Guide Request to Private Trip 🔐",
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


        public async Task<ApiResponse> Handle(AcceptPivRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var item = await ReadGenericRepo.GetByIdAsync(request.RequestId);

                if (item == null)
                    return new ApiResponse(404);

                if (item.Accept == true)
                    return new ApiResponse(StatusCodes.Status204NoContent, "some one acc");


                bool isAlreadyAcceptedByAnother = await ReadGenericRepo.GetAll()
                    .AnyAsync(x => x.PrivateTripId == item.PrivateTripId && x.Accept == true, cancellationToken);


                if (isAlreadyAcceptedByAnother)
                {
                    return new ApiResponse(
                    StatusCodes.Status400BadRequest,
                    "Sorry, this trip has already been accepted by another tour guide."
                    );
                }
                var trip = await Sender.Send(new GetPrivTripSpecQuery(new TripFilter
                {
                    Id = item.PrivateTripId,
                })) as ApiResultResponse<PrivateTemplateTrip>;
                if (trip?.Data == null)
                    return new ApiResponse(404);

                item.Accept = true;
                item.AcceptedAt = DateTime.UtcNow;

                var updatetrip = await Sender.Send(new AddTourguideToPivTrip(item.TourGuideId, item.PrivateTripId));
                if (updatetrip.statusCode != 200)
                    return updatetrip;
                //var res = await Sender.Send(new AddTourguideToPubTrip(item.TourGuideId, item.PrivateTripId));
                //if (res.statusCode != 200)
                //    return res;

                var resupdt = await Sender.Send(new UpdatePrivTripStatus(trip.Data.CreatedById, item.PrivateTripId, TripStatus.GuideAssigned));
                if (resupdt.statusCode != 200)
                    return resupdt;

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

