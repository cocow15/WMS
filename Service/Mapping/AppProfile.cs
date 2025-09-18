using AutoMapper;
using ApplicationTest.Dtos;
using ApplicationTest.Entities;

namespace ApplicationTest.Mapping;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<Product, ProductView>()
          .ForMember(d => d.Brand, m => m.MapFrom(s => s.Brand != null ? s.Brand.Name : null))
          .ForMember(d => d.Category, m => m.MapFrom(s => s.Category != null ? s.Category.Name : null));

        CreateMap<Brand, BrandView>();
        CreateMap<Category, CategoryView>();
    }
}
