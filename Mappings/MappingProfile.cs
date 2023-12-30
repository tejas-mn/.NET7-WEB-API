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

            //if primitive types are nullable don't map
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<decimal?, decimal>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);

            //srcMember properties must be declared nullable ? for this to work
            CreateMap<CreateItemRequestDto, Product>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => {
                return srcMember!=null;
            }));

            CreateMap<CreateItemRequestDto, ItemDto>();

            CreateMap<CreateItemRequestDto, CreateItemResponseDto>();

            CreateMap<Product, CreateItemResponseDto>();

        }
    }
}
