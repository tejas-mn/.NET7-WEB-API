using System.ComponentModel.DataAnnotations;

namespace asp_net_web_api.API.DTO
{
    public class CreateItemRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Sku { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }

    }
}