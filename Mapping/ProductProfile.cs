using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using AutoMapper;

namespace APIEcommerce.Mapping {

    public class ProductProfile : Profile {

        public ProductProfile() {

            //Destino y opcion del ForMember es para incluir el nombre de la categoria en el DTO.
            CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.categoryName, opt => opt.MapFrom(src => src.category.name))
            .ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
        }

    }

}

