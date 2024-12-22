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
            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.ImageUrl, otp => otp.MapFrom(src => src.ImageUrl))
                .ReverseMap();
            CreateMap<Product, ProductListDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ReverseMap();
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderDetailsDto, opt => opt.MapFrom(src => src.OrderDetail));
            CreateMap<Payment, PaymentDto>().ReverseMap();

            CreateMap<CreateProductCommand, Product>().ReverseMap();
            CreateMap<UpdateProductCommand, Product>().ReverseMap();
            CreateMap<UpdateCategoryCommand, Category>().ReverseMap();
            CreateMap<CreateCategoryCommand, Category>().ReverseMap();
        }
    }
}
