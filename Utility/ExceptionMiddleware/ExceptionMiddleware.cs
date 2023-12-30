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
        private readonly TokenStoreCache cc;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env, IConfiguration config, TokenStoreCache cx){
            _logger = logger;
            _env = env;
            _config = config;
            cc = cx;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {   
            LogRequest(context);
        
            try{
                var checkEndpoints = new[]{"api/Products", "api/Auth/logout", "api/Auth/refresh", "api/Categories", "api/AuthManager"};
                
                if(checkEndpoints.Any(p => context.Request.Path.ToString().Contains(p))){
                    var userAccesstoken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                    if (!cc.Store.ContainsKey(userAccesstoken)) throw new UnauthorizedAccessException("Already Logged out or invalid jwt exception");
                    validateJWT(context);
                }
                await next(context);
                
                if(context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    var res = new {StatusCode = 401, Message = "You are Unauthorized!"};
                    await context.Response.WriteAsync(res.ToString());
                }
                if(context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    var res = new {StatusCode = 403, Message = "Forbidden. You dont have access to this resource!"};
                    await context.Response.WriteAsync(res.ToString());
                }
                
                LogResponse(context);
            }
            catch (Exception ex){
                 _logger.LogError(ex, ex.Message);

                if(ex.InnerException!=null) _logger.LogError("Inner Exception: " + ex.InnerException.Message.Substring(18, 24));

                Fault f = new FaultFactory().GetFault(ex);
                
                if(_env.IsDevelopment()) f.ErrorTrace = null;

                context.Response.StatusCode = (int)f.StatusCode;
                context.Response.ContentType = "application/json";
               
                await context.Response.WriteAsync(f.ToString());
            }
        }

        private async void LogRequest(HttpContext context)
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
