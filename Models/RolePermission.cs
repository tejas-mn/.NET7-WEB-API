using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_net_web_api.API.Models
{
    public class RolePermission 
    {
        [Required]
        [Key]
        [ForeignKey("Role")]
        public int RoleId {get;set;}
        
        [Required]
        [Key]
        [ForeignKey("Permission")]
        public int PermissionId {get; set;}
        
        public virtual Role Role { get; set; }
        
        public virtual Permission Permission { get; set; }
    }
}