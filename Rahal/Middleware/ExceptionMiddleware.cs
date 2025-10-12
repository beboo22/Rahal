using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Domain.BaseResponce;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Rahal.Middleware
{
    internal class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                var statusCode = ex switch
                {
                    // Client-side issues (4xx)
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,                   // 404
                    ArgumentNullException => (int)HttpStatusCode.BadRequest,                // 400
                    ArgumentException => (int)HttpStatusCode.BadRequest,                    // 400
                    FormatException => (int)HttpStatusCode.BadRequest,                      // 400
                    InvalidOperationException => (int)HttpStatusCode.Conflict,              // 409
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,        // 401
                    SecurityException => (int)HttpStatusCode.Forbidden,                     // 403
                    TimeoutException => (int)HttpStatusCode.RequestTimeout,                 // 408
                    NotSupportedException => (int)HttpStatusCode.MethodNotAllowed,          // 405
                    DirectoryNotFoundException => (int)HttpStatusCode.NotFound,             // 404
                    FileNotFoundException => (int)HttpStatusCode.NotFound,                  // 404
                    EndOfStreamException => (int)HttpStatusCode.BadRequest,                 // 400

                    // Data & persistence issues
                    DbUpdateConcurrencyException => (int)HttpStatusCode.Conflict,           // 409
                    DbUpdateException => (int)HttpStatusCode.BadRequest,                    // 400
                    SqlException => (int)HttpStatusCode.ServiceUnavailable,                 // 503

                    // Fallback
                    _ => (int)HttpStatusCode.InternalServerError                            // 500
                };

                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";

                object apiResponse = _env.IsDevelopment()
                    ? new ApiResultResponse<string>(statusCode, ex.StackTrace ?? "", ex.Message)
                    : new ApiResponse(statusCode, ex.Message);

                var json = JsonSerializer.Serialize(apiResponse);

                await httpContext.Response.WriteAsync(json);
            }
        }
    }

}
