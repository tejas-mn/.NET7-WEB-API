using System.Text.Json.Serialization;

namespace asp_net_web_api.API.Models
{
    public class BaseModel
    {
        public int Id { get; set; }
        
        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}