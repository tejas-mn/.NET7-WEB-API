using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using AutoMapper;

namespace asp_net_web_api.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InventoryItem, ItemDto?>();

            CreateMap<Category, CategoryDto>();

            CreateMap<CreateItemRequestDto, InventoryItem>();

            CreateMap<CreateItemRequestDto, ItemDto>();

            CreateMap<CreateItemRequestDto, CreateItemResponseDto>();

        }
    }
}
