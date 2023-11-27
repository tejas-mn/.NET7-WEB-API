using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using asp_net_web_api.API.Utility;
using asp_net_web_api.API.ErrorHandling;
using System.Net;

namespace asp_net_web_api.API.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env){
            _logger = logger;
            Env = env;
        }

        public IHostEnvironment Env { get; }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {   
            LogRequest(context);
            
            #region comment
            //Get request stream and reset the position of this stream
            // Stream requestBodyStream = context.Request.Body;
            // string requestBody = string.Empty;

            // using (StreamReader sr = new StreamReader(requestBodyStream)){
            //     requestBody = await sr.ReadToEndAsync();
            // }

            // _logger.LogInformation("req body: " + requestBody);


            // _logger.LogInformation("req body: "+requestBody);
            #endregion 
        
            try{
                await next(context);
                LogResponse(context);
            }
            catch (Exception ex){
                _logger.LogError(ex, ex.Message);
                if(ex.InnerException!=null)
                    _logger.LogInformation("xxxxxxxxxx: " + ex.InnerException.Message.Substring(18, 24));

                Fault f;
                var exceptionType = ex.GetType();
                string message = "Internal server error";

                HttpStatusCode StatusCode = HttpStatusCode.InternalServerError;

                if(exceptionType==typeof(ItemNotFoundException)){
                    StatusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                }

                if(exceptionType == typeof(CategoryNotFoundException)){
                    StatusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                }

                if(Env.IsDevelopment()){
                    f = new Fault(message);
                }else{
                    f = new Fault(message, ex.StackTrace.ToString());
                }
                
                context.Response.StatusCode = (int)StatusCode;
                context.Response.ContentType = "application/json";
               
                await context.Response.WriteAsync(f.ToString());
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
        public static IApplicationBuilder UseCustomExceptionHandlingMiddleware(this IApplicationBuilder app){
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
