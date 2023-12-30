using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Models
{
    public class UserRole
    {
        [Required]
        [Key]
        [ForeignKey("Role")]
        public int RoleId {get;set;}
        
        [Required]
        [Key]
        [ForeignKey("User")]
        public int UserId {get; set;}

        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual Role Role { get; set; }
    }
}