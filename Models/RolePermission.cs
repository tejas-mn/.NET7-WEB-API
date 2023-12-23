using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.Models
{
    public class RolePermission 
    {
        [Required]
        public int RoleId {get;set;}
        [Required]
        public int PermissionId {get; set;}
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}