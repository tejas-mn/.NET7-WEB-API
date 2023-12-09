using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace asp_net_web_api.API.DTO
{
    public class UserDto : BaseDto
    {
        [Required]
        public required string Name {get; set;}
        public DateTime? CreatedAt { get; set; }
    }
}