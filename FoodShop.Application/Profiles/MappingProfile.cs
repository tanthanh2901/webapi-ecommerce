using AutoMapper;
using FoodShop.Application.Dto;
using FoodShop.Domain.Entities;
using FoodShop.Application.Feature.Categories.Commands.CreateCategory;
using FoodShop.Application.Feature.Categories.Commands.UpdateCategory;
using FoodShop.Application.Feature.Products.Commands.CreateProduct;
using FoodShop.Application.Feature.Products.Commands.UpdateProduct;

namespace FoodShop.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Product, ProductDetailsDto>().ReverseMap();
            CreateMap<Product, ProductListDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();
            CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetail));

            CreateMap<CreateProductCommand, Product>().ReverseMap();
            CreateMap<UpdateProductCommand, Product>().ReverseMap();
            CreateMap<UpdateCategoryCommand, Category>().ReverseMap();
            CreateMap<CreateCategoryCommand, Category>().ReverseMap();
        }
    }
}
