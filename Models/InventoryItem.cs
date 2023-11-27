using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace asp_net_web_api.API.Models
{
    public class InventoryItem : BaseModel
    {
        [Required]
        public string Sku { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category? Category { get; set; }
    }
}