using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.Models
{
    public class UserRole
    {
        [Required]
        public int RoleId {get;set;}
        
        [Required]
        public int UserId {get; set;}

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}