using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace asp_net_web_api.API.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger){
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {   
            LogRequest(context);
            LogResponse(context);
            
            try{
                await next(context);
            }
            catch (Exception ex){
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "An unexpected error occurred." }));
            }
        }

        private void LogRequest(HttpContext context)
        {
            var request = context.Request;

            var requestLog = new StringBuilder();
            requestLog.AppendLine("Incoming Request:");
            requestLog.AppendLine($"HTTP {request.Method} {request.Path}");
            requestLog.AppendLine($"Host: {request.Host}");
            requestLog.AppendLine($"Content-Type: {request.ContentType}");
            requestLog.AppendLine($"Content-Length: {request.ContentLength}");

            _logger.LogInformation(requestLog.ToString());
        }

        private void LogResponse(HttpContext context)
        {
            var response = context.Response;

            var responseLog = new StringBuilder();
            responseLog.AppendLine("Outgoing Response:");
            responseLog.AppendLine($"HTTP {response.StatusCode}");
            responseLog.AppendLine($"Content-Type: {response.ContentType}");
            responseLog.AppendLine($"Content-Length: {response.ContentLength}");

            _logger.LogInformation(responseLog.ToString());
        }

    }

    public static class ExceptionHandlingMiddlewareExtensions{
        public static IApplicationBuilder UseCustomExceptionHandlingMiddleware(this IApplicationBuilder builder){
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
