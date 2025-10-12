using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Infrastructure.Abestraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Fetures.Authentication.Command
{
    public class AuthenticationCommandHandler : ICommandHandler<ResetRequestCommand, ApiResponse>,
                                                ICommandHandler<ResetPasswordCommand, ApiResponse>,
                                                ICommandHandler<signUpCommand, ApiResponse>,
                                                ICommandHandler<LogOutCommand, ApiResponse>
    {
        private IWriteUnitOfWork _uow;
        private IWriteUserRepo _wur;
        private IReadGenericRepo<User> _rur;
        private IWriteGenericRepo<RefreshToken> _wgrRepo;
        private IWriteGenericRepo<UserRole> _WURR;
        private IReadGenericRepo<RefreshToken> _rgrRepo;
        private IWriteGenericRepo<PasswordResetToken> _wgpRepo;
        private IReadGenericRepo<PasswordResetToken> _rgpRepo;
        private IEmailService _emailService;
        public AuthenticationCommandHandler(IWriteUserRepo writeUserRepo, IWriteUnitOfWork uow, IWriteGenericRepo<RefreshToken> wurRepo, IReadGenericRepo<RefreshToken> rurRepo, IReadGenericRepo<User> rur, IEmailService emailService, IWriteGenericRepo<PasswordResetToken> wgpRepo, IReadGenericRepo<PasswordResetToken> rgpRepo, IWriteGenericRepo<UserRole> wURR)
        {
            _wur = writeUserRepo;
            _uow = uow;
            _wgrRepo = wurRepo;
            _rgrRepo = rurRepo;
            _rur = rur;
            _emailService = emailService;
            _wgpRepo = wgpRepo;
            _rgpRepo = rgpRepo;
            _WURR = wURR;
        }

        public async Task<ApiResponse> Handle(signUpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _uow.BeginTransiaction();

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
                    //Bio = request.signUpData.Bio?.Trim(),
                    //Ssn = request.signUpData.Ssn.Trim(),
                    Age = request.signUpData.Age,
                    PasswordHash = request.signUpData.Password,
                    phoneNumbers = request.signUpData.phoneNumbers?.Select(p => new PhoneNumber
                    {
                        CountryCode = p.CountryCode,
                        Number = p.Number,
                        HasWhatsApp = p.HasWhatsApp,
                    }).ToList() ?? new List<PhoneNumber>(),
                };

                var hasher = new PasswordHasher<User>();
                newUser.PasswordHash = hasher.HashPassword(newUser, request.signUpData.Password);

                // 🔹 Inject roles here
                //foreach (var roleId in request.signUpData.RoleIds)
                //{
                //    newUser.Roles.Add(new UserRole
                //    {
                //        RoleId = roleId,
                //        User = newUser
                //    });
                //}
                await _wur.AddAsync(newUser);
                var userRoles = request.signUpData.RoleIds.Select(roleId => new UserRole
                {
                    RoleId = roleId,
                    User = newUser  // EF Core will resolve the foreign key automatically
                }).ToList();
                await _WURR.AddRangAsync(userRoles);


                await _uow.SaveChangesAsync();
                return new ApiResponse((int)HttpStatusCode.OK, "User registered successfully.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new ApiResponse((int)HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        //public async Task<ApiResponse> Handle(LogOutCommand request, CancellationToken cancellationToken)
        //{
        //    var storedToken = await _rurRepo.GetAll()
        //                    .Include(s => s.Traveler)
        //                    .Include(s => s.TourGuid)
        //                    .Include(s => s.TravelerCompany)
        //                    .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);



        //    if (storedToken != null)
        //    {
        //        storedToken.Revoked = true;
        //        storedToken./*how can i handel this*/.LastLogoutTime = DateTime.UtcNow; // Update last logout time
        //        await _uow.BeginTransiaction();
        //        await _wurRepo.UpdateAsync(storedToken, storedToken.Id);
        //        await _uow.SaveChangesAsync();
        //    }
        //}






        public async Task<ApiResponse> Handle(ResetRequestCommand request, CancellationToken cancellationToken)
        {
            var user = await _rur.GetAll().FirstOrDefaultAsync(u => u.Email == request.ResetRequestDto.Email);

            if (user == null)
                return new ApiResponse((int)HttpStatusCode.NotFound); // Don't reveal existence

            await _uow.BeginTransiaction();

            var resetToken = Guid.NewGuid().ToString(); // better: use secure RNG
            var pssreset = new PasswordResetToken
            {
                UserId = user.Id,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            int role = -1;



            await _wgpRepo.AddAsync(pssreset);
            await _uow.SaveChangesAsync();

            // include role in the reset link
            var resetLink = $"https://localhost:7030/reset-password?token={resetToken}&role={role}";
            _emailService.SendEmail(user.Email, "Password Reset Request",
               $"Click the link to reset your password: {resetLink}");

            return new ApiResponse(200, "Check your email for password reset instructions.");
        }

        public async Task<ApiResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            await _uow.BeginTransiaction();
            var query = _rgpRepo.GetAll();

            var tokenEntity = await query
                .Include(r => r.User)
                    .ThenInclude(rt => rt.RefreshTokens)
                .FirstOrDefaultAsync(r => r.Token == request.ResetPasswordDto.Token && r.ExpiresAt > DateTime.UtcNow, cancellationToken);

            if (tokenEntity == null) return new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid token");

            var user = tokenEntity.User;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ResetPasswordDto.NewPassword);
            user.LastPasswordResetTime = DateTime.UtcNow;

            foreach (var rt in user.RefreshTokens)
                rt.Revoked = true;
            await _wur.UpdateAsync(user, user.Id);
            await _wgpRepo.DeleteAsync(tokenEntity.Id); // one-time use
            await _uow.SaveChangesAsync();



            return new ApiResponse(200, "Password reset successful");
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

            await _uow.BeginTransiaction();
            await _wgrRepo.UpdateAsync(storedToken, storedToken.Id);
            await _uow.SaveChangesAsync();

            return new ApiResponse((int)HttpStatusCode.OK, "Logged out successfully.");
        }


        //public async Task<ApiResponse> Handle(LogOutCommand request, CancellationToken cancellationToken)
        //{
        //    var storedToken = await _rurRepo.GetAll()
        //        .Include(s => s.Traveler)
        //        .Include(s => s.TourGuid)
        //        .Include(s => s.TravelerCompany)
        //        .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        //    if (storedToken == null)
        //        return new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid refresh token.");

        //    storedToken.Revoked = true;

        //    // 🔑 switch expression on the user type
        //    switch (storedToken)
        //    {
        //        case { Traveler: not null }:
        //            storedToken.Traveler.LastLogoutTime = DateTime.UtcNow;
        //            break;
        //        case { TourGuid: not null }:
        //            storedToken.TourGuid.LastLogoutTime = DateTime.UtcNow;
        //            break;
        //        case { TravelerCompany: not null }:
        //            storedToken.TravelerCompany.LastLogoutTime = DateTime.UtcNow;
        //            break;
        //    }

        //    await _uow.BeginTransiaction();
        //    await _wurRepo.UpdateAsync(storedToken, storedToken.Id);
        //    await _uow.SaveChangesAsync();

        //    return new ApiResponse((int)HttpStatusCode.OK, "Logged out successfully.");
        //}


    }


    //public class AuthenticationCommandHandler : ICommandHandler<LogOutCommand, ApiResponse>
    //{
    //    private IWriteUnitOfWork _uow;
    //    private IWriteGenericRepo<RefreshToken> _wgrRepo;
    //    private IReadGenericRepo<RefreshToken> _rgrRepo;

    //    public AuthenticationCommandHandler(IReadGenericRepo<RefreshToken> rgrRepo, IWriteGenericRepo<RefreshToken> wgrRepo, IWriteUnitOfWork uow)
    //    {
    //        _rgrRepo = rgrRepo;
    //        _wgrRepo = wgrRepo;
    //        _uow = uow;
    //    }

    //    public async Task<ApiResponse> Handle(LogOutCommand request, CancellationToken cancellationToken)
    //    {
    //        var storedToken = await _rgrRepo.GetAll()
    //            .Include(s => s.Traveler)
    //            .Include(s => s.TourGuide)
    //            .Include(s => s.TravelCompany)
    //            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

    //        if (storedToken == null)
    //            return new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid refresh token.");

    //        // revoke token
    //        storedToken.Revoked = true;

    //        //  unify user reference (Traveler, TourGuid, or TravelerCompany)
    //        User? user = storedToken.Traveler as User
    //          ?? storedToken.TourGuide as User
    //          ?? storedToken.TravelCompany as User;


    //        if (user != null)
    //            user.LastLogoutTime = DateTime.UtcNow;

    //        await _uow.BeginTransiaction();
    //        await _wgrRepo.UpdateAsync(storedToken, storedToken.Id);
    //        await _uow.SaveChangesAsync();

    //        return new ApiResponse((int)HttpStatusCode.OK, "Logged out successfully.");
    //    }

    //}

}

