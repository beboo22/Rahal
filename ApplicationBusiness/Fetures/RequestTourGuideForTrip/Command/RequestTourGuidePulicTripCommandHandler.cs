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
    public record RequestTourGuidePubTripCommand(int Trip, List<int> TourguideIds) : ICommand<ApiResponse>;
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
            foreach (var TourguideId in request.TourguideIds)
            {

                var cheacktour = await Sender.Send(new CheckTourguideExsist(TourguideId));
                if (cheacktour.statusCode != StatusCodes.Status302Found)
                    return new ApiResponse(StatusCodes.Status404NotFound, "Can't found tourguide");

            }

            //var cheacktrip = await Sender.Send(new CheckPubTripExsist(request.Trip));
            //if (cheacktrip.statusCode != StatusCodes.Status302Found)
            //    return new ApiResponse(StatusCodes.Status404NotFound, "Can't found trip");

            var trip = await Sender.Send(new GetPubTripSpecQuery(new TripFilter
            {
                Id = request.Trip,
            })) as ApiResultResponse<TemplateTrip>;
            if (trip?.Data == null)
                return new ApiResponse(404);

            var item = new List<RequestTourGuidePulicTrip>();


            foreach (var TourguideId in request.TourguideIds)
            {
                item.Add(new RequestTourGuidePulicTrip
                {

                    PublicTripId = request.Trip,
                    TourGuideId = TourguideId,
                });
            }
            try
            {
                await writeUnitOfWork.BeginTransactionAsync();
                await WriteGenericRepo.AddRangAsync(item);
                await writeUnitOfWork.SaveChangesAsync();
                await writeUnitOfWork.CommitAsync();


                foreach (var TourguideId in request.TourguideIds)
                {
                    await Sender.Send(
                    new SendGuideRequestNotificationCommand(
                        TourguideId.ToString(),
                        "New Guide Request 🧭",
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
                    return new ApiResponse(StatusCodes.Status400BadRequest, "عذراً، تم قبول هذه الرحلة بالفعل من قِبل مرشد سياحي آخر.");
                }


                item.Accept = true;
                item.AcceptedAt = DateTime.UtcNow;
                var updatetrip = await Sender.Send(new AddTourguideToPubTrip(item.TourGuideId, item.PublicTripId));
                if (updatetrip.statusCode != 200)
                    return updatetrip;
                var res = await Sender.Send(new AddTourguideToPubTrip(item.TourGuideId, item.PublicTripId));
                if (res.statusCode != 200)
                    return res;
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
