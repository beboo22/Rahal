using Application.Abestraction;
using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Infrastructure.Abestraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Abestraction;
using System.Net;

namespace Application.Fetures.Authentication.Query
{
    public class AuthenticationQueryHandler : IQueryHandler<LoginQuery, ApiResponse>,
                                              IQueryHandler<RefreshTokenModel, ApiResponse>
    {
        private IWriteUnitOfWork _out;
        private IReadGenericRepo<User> _RUR;
        private IWriteGenericRepo<User> _Wrepo;
        private IAuthentication _auth;
        private IEmailService emailService;

        public AuthenticationQueryHandler(IReadGenericRepo<User> rUR, IAuthentication auth, IWriteUnitOfWork @out, IWriteGenericRepo<User> wrepo, IEmailService emailService)
        {
            _RUR = rUR;
            _auth = auth;
            _out = @out;
            _Wrepo = wrepo;
            this.emailService = emailService;
        }
        private string BuildOtpEmailBody(string otpCode)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Rahal - Verification Code</title>

<style>
body {{
    margin: 0;
    padding: 0;
    background-color: #f4f6f9;
    font-family: 'Segoe UI', Tahoma, Arial, sans-serif;
}}

.wrapper {{
    width: 100%;
    padding: 40px 0;
}}

.container {{
    max-width: 600px;
    margin: auto;
    background: #ffffff;
    border-radius: 14px;
    overflow: hidden;
    box-shadow: 0 8px 20px rgba(0,0,0,0.08);
}}

.header {{
    background: linear-gradient(135deg, #1e3c72, #2a5298);
    padding: 30px;
    text-align: center;
    color: #ffffff;
}}

.header h1 {{
    margin: 0;
    font-size: 28px;
    letter-spacing: 1px;
}}

.header p {{
    margin: 8px 0 0;
    font-size: 14px;
    opacity: 0.9;
}}

.content {{
    padding: 35px 30px;
    color: #333333;
}}

.content h2 {{
    margin-top: 0;
    font-size: 20px;
    color: #1e3c72;
}}

.content p {{
    font-size: 15px;
    line-height: 1.8;
    margin: 12px 0;
}}

.otp-container {{
    text-align: center;
    margin: 30px 0;
}}

.otp-code {{
    display: inline-block;
    background: linear-gradient(135deg, #ff512f, #dd2476);
    color: #ffffff;
    font-size: 26px;
    font-weight: bold;
    padding: 15px 35px;
    border-radius: 10px;
    letter-spacing: 8px;
}}

.expiry {{
    text-align: center;
    font-size: 14px;
    color: #666;
}}

.warning {{
    margin-top: 25px;
    padding: 14px;
    background-color: #fff4f4;
    border: 1px solid #ffd6d6;
    border-radius: 8px;
    font-size: 13px;
    color: #c0392b;
}}

.footer {{
    background-color: #f4f6f9;
    text-align: center;
    padding: 20px;
    font-size: 12px;
    color: #777;
}}

.footer strong {{
    color: #1e3c72;
}}

@media screen and (max-width: 600px) {{
    .content {{
        padding: 25px 18px;
    }}

    .otp-code {{
        font-size: 22px;
        padding: 12px 25px;
    }}
}}
</style>
</head>

<body>
<div class=""wrapper"">
    <div class=""container"">
        
        <div class=""header"">
            <h1>Rahal</h1>
            <p>Your Travel & Experience Platform</p>
        </div>

        <div class=""content"">
            <h2>Email Verification</h2>
            
            <p>Hello,</p>
            
            <p>
                To complete your verification process with <strong>Rahal</strong>,
                please use the One-Time Password (OTP) below:
            </p>

            <div class=""otp-container"">
                <div class=""otp-code"">{otpCode}</div>
            </div>

            <p class=""expiry"">
                This code is valid for <strong>3 minutes only</strong>.
            </p>

            <div class=""warning"">
                ⚠️ For your security, do not share this code with anyone. 
                Rahal will never ask you for your OTP.
            </div>

            <p>
                If you did not request this code, you may safely ignore this email.
            </p>

            <p style=""margin-top:30px;"">
                Thank you for choosing <strong>Rahal</strong>. ✈️
            </p>
        </div>

        <div class=""footer"">
            © {DateTime.UtcNow.Year} Rahal. All rights reserved.<br/>
            This is an automated security message related to your account.
        </div>

    </div>
</div>
</body>
</html>";
        }

        public async Task<ApiResponse> Handle(
      LoginQuery request,
      CancellationToken cancellationToken)
        {
            var user = await _RUR.GetAll()
                .FirstOrDefaultAsync(
                    x => x.Email == request.loginDto.Email,
                    cancellationToken);

            if (user == null)
                return new ApiResponse(404, "User not found");


            // Check OTP Delay
            if (!user.CanRequestOtp(out var message, out double remaining))
                return new ApiResultResponse<double>(429, remaining, message);


            // Special Email

            user.GenerateOtp();



            // Register Request
            DateTime delayM = user.RegisterOtpRequest();


            await _out.BeginTransactionAsync();

            await _Wrepo.UpdateAsync(user, user.Id);

            await _out.SaveChangesAsync();
            await _out.CommitAsync();



            var htmlBody =
                BuildOtpEmailBody(user.OtpCode);


            emailService.SendEmail(
                user.Email,
                "Login details and Verification",
                htmlBody,
                isHtml: true
            );


            return new ApiResultResponse<DateTime>(
                200, delayM,
                $"OTP sent successfully and Next Otp Allowed At {delayM}");
        }

        public async Task<ApiResponse> Handle(RefreshTokenModel request, CancellationToken cancellationToken)
        {
            var newToken = await _auth.RefreshTokenAsync(request.refreshtoken);
            if (!newToken.HasValue)
                return new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid or expired refresh token");

            return new JwtAuthResponse((int)HttpStatusCode.OK, newToken.Value);
        }

    }
    //public Task<ApiResponse> Handle(LogOutQuery request, CancellationToken cancellationToken)
    //{
    //    var storedToken = await _context.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken);
    //    if (storedToken != null)
    //    {
    //        storedToken.Revoked = true;
    //        storedToken.User.LastLogoutTime = DateTime.UtcNow; // Update last logout time
    //        await _context.SaveChangesAsync();
    //    }
    //}

    //public class AuthenticationQueryHandler : IQueryHandler<RefreshTokenModel, ApiResponse>
    //{
    //    private IAuthentication _auth;

    //    public AuthenticationQueryHandler(IAuthentication auth)
    //    {
    //        _auth = auth;
    //    }

    //    public async Task<ApiResponse> Handle(RefreshTokenModel request, CancellationToken cancellationToken)
    //    {
    //        var newToken = await _auth.RefreshTokenAsync(request.refreshtoken);
    //        if (!newToken.HasValue)
    //            return new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid or expired refresh token");

    //        return new JwtAuthResponse((int)HttpStatusCode.OK, newToken.Value);
    //    }

    //}

}
