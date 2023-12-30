using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.DTO
{
    public class CreateItemRequestDto
    {
        [Required]
        public int Id { get; set; }

        public string? Sku { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } // ?-required if this has to be ignored if null by automapper

        public decimal? Price { get; set; } 

        public bool? IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }

    }
}