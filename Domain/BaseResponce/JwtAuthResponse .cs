using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BaseResponce
{
    public class JwtAuthResponse:ApiResponse
    {
        public (string AccessToken, string RefreshToken, DateTime Expiration) Token { get; set; }
        public JwtAuthResponse(int Scode, (string AccessToken, string RefreshToken, DateTime Expiration) _token, string? msg = null) : base(Scode, msg)
        {
            Token = _token;
        }

    }
}
