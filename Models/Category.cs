using System.Text.Json.Serialization;

namespace asp_net_web_api.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual List<InventoryItem>? InventoryItems { get; set; } 
    }
}