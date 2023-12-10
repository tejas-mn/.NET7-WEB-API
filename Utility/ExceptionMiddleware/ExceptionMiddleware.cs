using System.Text;
using asp_net_web_api.API.Utility;
using asp_net_web_api.API.ErrorHandling;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace asp_net_web_api.API.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private IHostEnvironment _env { get; }
        public IConfiguration _config { get; }

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env, IConfiguration config){
            _logger = logger;
            _env = env;
            _config = config;
        }

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
                if(context.Request.Path.ToString().Contains("api/Inventory"))  validateJWT(context);
                await next(context);
                LogResponse(context);
            }
            catch (Exception ex){
                _logger.LogError(ex, ex.Message);

                if(ex.InnerException!=null)
                    _logger.LogError("Inner Exception: " + ex.InnerException.Message.Substring(18, 24));

                Fault f;
                var exceptionType = ex.GetType();
                string message = ex.Message;

                HttpStatusCode StatusCode = HttpStatusCode.InternalServerError;

                if(exceptionType==typeof(ItemNotFoundException)){
                    StatusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                }

                if(exceptionType == typeof(CategoryNotFoundException)){
                    StatusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                }

                if(_env.IsDevelopment()){
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
            // var userAccesstoken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // _logger.LogInformation("ACCESS TOKEN: " + userAccesstoken);
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

        private  void validateJWT(HttpContext context)
        {
            var userAccesstoken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var secret = _config.GetSection("AppSettings:Key").Value; 
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(userAccesstoken, tokenValidationParameters, out _);
                }
                catch (Exception ex)
                {
                    // Fault f = new Fault("Invalid jwt token!");
                    // context.Response.ContentType = "application/json";
                    // await context.Response.WriteAsync(f.ToString());
                    throw ex;
                }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions{
        public static IApplicationBuilder UseCustomExceptionHandlingMiddleware(this IApplicationBuilder app){
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
