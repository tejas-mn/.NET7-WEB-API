using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.DTO
{
    public class ForgotPasswordRequestDto {
        public string Name {get; set;}
        
        [Required]
        public string NewPassword {get;set;}
    }
}