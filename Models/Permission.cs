
using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.Models
{
    public class Permission : BaseModel
    {
        [Required]
        public string Name {get; set;}

        public virtual IEnumerable<RolePermission> RolePermissions {get;set;}
    }
}