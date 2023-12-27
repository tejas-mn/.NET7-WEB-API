using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace asp_net_web_api.API.DTO
{
    public class UserDto : BaseDto
    {
        public int Id {get; set;}

        [Required]
        public required string Name {get; set;}

        public List<string>? Roles {get;set;}

        public HashSet<string>? Permissions {get;set;}

        public DateTime? CreatedAt { get; set; }
    }
}