using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace asp_net_web_api.API.Models
{
    public class User : BaseModel
    {
        [Required]
        public required string Name {get; set;}
        [Required]
        public required byte[] Password {get; set;}
        public required byte[] PasswordKey {get; set;}
    }
}