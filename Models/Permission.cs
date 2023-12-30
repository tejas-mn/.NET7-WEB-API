
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace asp_net_web_api.API.Models
{
    public class Permission : BaseModel
    {
        [Required]
        public string Name {get; set;}
        
        [JsonIgnore]
        public virtual IEnumerable<RolePermission> RolePermissions {get;set;}
    }
}