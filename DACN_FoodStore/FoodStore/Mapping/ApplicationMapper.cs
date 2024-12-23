using AutoMapper;
using FoodStore.DTO;
using FoodStore.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FoodStore.Mapping
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper() { 
        
            CreateMap<Food, FoodDTO>().ReverseMap();
            CreateMap<FoodCategory, FoodCategoryDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<Table, TableDTO>().ReverseMap();
            CreateMap<QRCode, QRCodeDTO>().ReverseMap();
            CreateMap<Buffet, BuffetDTO>().ReverseMap();
            CreateMap<BuffetDetail, BuffetDetailDTO>().ReverseMap();

        }

    }
}
