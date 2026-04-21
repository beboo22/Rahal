using Application.Abestraction;
using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Infrastructure.Abestraction;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Fetures.Authentication.Command
{
    public class AuthenticationCommandHandler : ICommandHandler<VerifyOtpCommand, ApiResponse>,
                                                ICommandHandler<signUpCommand, ApiResponse>,
                                                ICommandHandler<IsUserExist, ApiResponse>,
                                                ICommandHandler<LogOutCommand, ApiResponse>,
                                                ICommandHandler<VerifiedUser, ApiResponse>

    {
        private IWriteUnitOfWork _uow;

        private IWriteUserRepo _wur;
        private IReadGenericRepo<User> _rur;
        private IWriteGenericRepo<RefreshToken> _wgrRepo;
        private IAuthentication authServ;

        private IWriteGenericRepo<UserRole> _WURR;
        private IReadGenericRepo<RefreshToken> _rgrRepo;

        public AuthenticationCommandHandler(IWriteUnitOfWork uow, IWriteUserRepo wur, IWriteGenericRepo<UserRole> wURR, IReadGenericRepo<User> rur, IAuthentication authServ, IWriteGenericRepo<RefreshToken> wgrRepo, IReadGenericRepo<RefreshToken> rgrRepo)
        {
            _uow = uow;
            _wur = wur;
            _WURR = wURR;
            _rur = rur;
            this.authServ = authServ;
            _wgrRepo = wgrRepo;
            _rgrRepo = rgrRepo;
        }


        public async Task<ApiResponse> Handle(signUpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransactionAsync();

                if (request.signUpData == null)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User data is required.");

                var exists = await _wur.ExistsAsync(request.signUpData.Email);
                if (exists)
                    return new ApiResponse((int)HttpStatusCode.BadRequest, "User with this email already exists.");

                var newUser = new User
                {
                    Email = request.signUpData.Email.Trim(),
                    Languages = request.signUpData.Languages?.Select(l => new Languages
                    {
                        Code = l.Code,
                        CreatedAt = DateTime.UtcNow
                    }).ToList() ?? new List<Languages>(),
                    LName = request.signUpData.LName.Trim(),
                    FName = request.signUpData.FName.Trim(),
                    Age = request.signUpData.Age,
                    phoneNumbers = request.signUpData.phoneNumbers?.Select(p => new PhoneNumber
                    {
                        CountryCode = p.CountryCode,
                        Number = p.Number,
                        HasWhatsApp = p.HasWhatsApp,
                    }).ToList() ?? new List<PhoneNumber>(),
                };


                await _wur.AddAsync(newUser);
                var userRoles = request.signUpData.Roles.Select(roleId => new UserRole
                {
                    RoleId = ((int)roleId) + 1,
                    User = newUser  // EF Core will resolve the foreign key automatically
                }).ToList();
                await _WURR.AddRangAsync(userRoles);


                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return new ApiResponse((int)HttpStatusCode.OK, "User registered successfully.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new ApiResponse((int)HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}");
            }
        }



        public async Task<ApiResponse> Handle(
    VerifyOtpCommand request,
    CancellationToken cancellationToken)
        {
            var user = await _rur.GetAll().Include(u => u.Roles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(
                    x => x.Email == request.Email,
                    cancellationToken);

            if (user == null)
                return new ApiResponse(404, "User not found");


            if (!user.ValidateOtp(request.Otp))
                return new ApiResponse(
                    400,
                    "Invalid or expired OTP");


            user.ClearOtp();
            await _uow.BeginTransactionAsync();

            await _wur.UpdateAsync(user, user.Id);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            var token =
                await authServ.CreateTokenAsync(user);


            return new ApiResultResponse<UserDto>(
                200,
                new UserDto
                {
                    FName = user.FName,
                    LName = user.LName,
                    Email = user.Email,
                    Age = user.Age,
                    BlockedEndDate = user.BlockedEndDate,
                    BlockedStartDate = user.BlockedStartDate,
                    FinancialBalance = user.FinancialBalance,
                    IsActive = user.IsActive,
                    IsBlocked = user.IsBlocked,
                    Isverified = user.Isverified,

                    Token = new Token
                    {
                        AccessToken = token.AccessToken,
                        ExpiryDate = token.Expiration,
                        RefreshToken = token.RefreshToken
                    }
                },
                "OTP verified successfully");
        }

        public async Task<ApiResponse> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _rgrRepo.GetAll()
                .Include(s => s.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (storedToken == null)
                return new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid refresh token.");

            // revoke token
            storedToken.Revoked = true;

            User? user = storedToken.User;


            if (user != null)
                user.LastLogoutTime = DateTime.UtcNow;

            await _uow.BeginTransactionAsync();
            await _wgrRepo.UpdateAsync(storedToken, storedToken.Id);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();


            return new ApiResponse((int)HttpStatusCode.OK, "Logged out successfully.");
        }

        public Task<ApiResponse> Handle(IsUserExist request, CancellationToken cancellationToken)
        {
            var exists = _rur.GetAll().Any(u => u.Id == request.UserId);
            if (exists)
                return Task.FromResult(new ApiResponse((int)HttpStatusCode.OK, "User exists."));
            else
                return Task.FromResult(new ApiResponse((int)HttpStatusCode.NotFound, "User does not exist."));
        }

        public async Task<ApiResponse> Handle(VerifiedUser request, CancellationToken cancellationToken)
        {
            var user = await _rur.GetByIdAsync(request.Id);
            if (user == null)
                return new ApiResponse(404);
            try
            {
                //await _uow.BeginTransactionAsync();

                user.Isverified = true;
                await _wur.UpdateAsync(user, request.Id);

                //await _uow.CommitAsync();
                return new ApiResponse(200, "update sucuss");

            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new ApiResponse((int)HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}

