using System.Text.Json.Serialization;
using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}