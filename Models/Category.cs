using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace asp_net_web_api.API.Models
{
    public class Category : BaseModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual List<Product>? Products { get; set; } 
    }
}