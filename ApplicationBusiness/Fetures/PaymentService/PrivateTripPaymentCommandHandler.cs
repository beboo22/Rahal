using Application.Abstraction.message;
using Application.Features.PaymentService;
using ApplicationBusiness.Services;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PaymentService
{
    internal class PrivateTripPaymentCommandHandler: ICommandHandler<PrivateTripCreatePayment, ApiResponse>
    {
        private IWriteGenericRepo<PaymentRequest> _Wrepo;
        private IReadGenericRepo<BookingPrivateTrip> _ROrepo;
        private IWriteUnitOfWork _unitOfWork;

        private IPaymobService paymobService;

        public PrivateTripPaymentCommandHandler(IPaymobService paymobService, IReadGenericRepo<BookingPrivateTrip> rOrepo, IWriteGenericRepo<PaymentRequest> wrepo, IWriteUnitOfWork unitOfWork)
        {
            this.paymobService = paymobService;
            _ROrepo = rOrepo;
            _Wrepo = wrepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> Handle(PrivateTripCreatePayment request, CancellationToken cancellationToken)
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
                EntityType = PaymentEntityType.PrivateTrip,
                IsPaid = false
            });

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();


            return new ApiResultResponse<string>(200, url);




        }
    }
}
