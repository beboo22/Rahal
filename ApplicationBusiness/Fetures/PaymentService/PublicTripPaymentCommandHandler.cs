using Application.Abstraction.message;
using Application.Abstraction.spacification;
using Application.Features.PaymentService;
using ApplicationBusiness.Fetures.PaymentService.Strategies;
using ApplicationBusiness.Services;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationBusiness.Fetures.PaymentService
{
    internal class PublicTripPaymentCommandHandler : ICommandHandler<PublicTripCreatePayment, ApiResponse>
    {
        private IWriteGenericRepo<PaymentRequest> _Wrepo;
        private IReadGenericRepo<BookingPublicTrip> _ROrepo;
        private IWriteUnitOfWork _unitOfWork;

        private IPaymobService paymobService;

        public PublicTripPaymentCommandHandler(IPaymobService paymobService, IReadGenericRepo<BookingPublicTrip> rOrepo, IWriteGenericRepo<PaymentRequest> wrepo, IWriteUnitOfWork unitOfWork)
        {
            this.paymobService = paymobService;
            _ROrepo = rOrepo;
            _Wrepo = wrepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> Handle(PublicTripCreatePayment request, CancellationToken cancellationToken)
        {


            var order = _ROrepo.GetAll().Where(x => x.Id == request.BookId && x.IsPaid == false && x.Canceled == false).Select(x => new Order
            {
                User = new User
                {
                    Email = x.User.Email,

                    phoneNumbers = x.User.phoneNumbers,
                    FName = x.User.FName,

                },
                TotalBookingPrice = x.TotalBookingPrice,
            }).FirstOrDefault();


            //var order = ;
            if (order == null)
                return new ApiResponse(404);

            order.ProviderRef = Guid.NewGuid().ToString();

            var url = await paymobService.InitiatePaymentAsync(order);

            if (url == null)
                return new ApiResponse(500);

            await _unitOfWork.BeginTransactionAsync();

            await _Wrepo.AddAsync(new PaymentRequest
            {
                ProviderRef = order.ProviderRef,
                EntityId = request.BookId,
                EntityType = PaymentEntityType.PublicTrip,
                IsPaid = false
            });

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();


            return new ApiResultResponse<string>(200, url);




        }
    }
}
