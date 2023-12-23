using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using AutoMapper;

namespace asp_net_web_api.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ItemDto?>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<CreateItemRequestDto, Product>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateItemRequestDto, ItemDto>();

            CreateMap<CreateItemRequestDto, CreateItemResponseDto>();

            CreateMap<Product, CreateItemResponseDto>();


        }
    }
}
