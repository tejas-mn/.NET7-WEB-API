using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.Models
{
    public class User : BaseModel
    {
        [Required]
        public string Name {get; set;}
        [Required]
        public string Password {get; set;}
    }
}