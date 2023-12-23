using System.Text;
using asp_net_web_api.API.Utility;
using asp_net_web_api.API.ErrorHandling;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using FluentValidation;

namespace asp_net_web_api.API.Middlewares
{
    public class FaultFactory {
        public Fault GetFault(Exception ex)
       {
            var exceptionType = ex.GetType();
            string message = ex.Message;

            HttpStatusCode StatusCode = HttpStatusCode.InternalServerError;

            if(exceptionType == typeof(ItemNotFoundException)){
                StatusCode = HttpStatusCode.NotFound;
                message = ex.Message;
            }

            if(exceptionType == typeof(CategoryNotFoundException)){
                StatusCode = HttpStatusCode.NotFound;
                message = ex.Message;
            }

            if(exceptionType == typeof(UnauthorizedAccessException)){
                StatusCode = HttpStatusCode.Unauthorized;
                message = ex.Message;
            }

            if(exceptionType == typeof(ValidationException)){
                StatusCode = HttpStatusCode.BadRequest;
                message = "One or More Validation Failed";
            }
         
            Fault f = new Fault(message, StatusCode, ex.StackTrace);
               
            if (ex is ValidationException validationException)
            {
                var x = validationException.Errors
                    .Select(error => $"{error.PropertyName}: {error.ErrorMessage}")
                    .ToList();
                
                f.Errors = x;
            }

            return f;
       }
    }
}
