using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.Models
{
    public class User : BaseModel
    {
        [Required]
        public required string Name {get; set;}
        [Required]
        public required string Password {get; set;}
    }
}