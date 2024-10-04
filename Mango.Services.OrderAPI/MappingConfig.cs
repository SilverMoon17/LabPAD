using AutoMapper;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;

namespace Mango.Services.OrderAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            // Маппинг для OrderHeader и CheckoutHeaderDto
            config.CreateMap<OrderHeader, CheckoutHeaderDto>()
                .ForMember(dest => dest.CartHeaderId, opt => opt.MapFrom(src => src.OrderHeaderId))
                .ForMember(dest => dest.CartDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ReverseMap()
                .ForMember(dest => dest.OrderHeaderId, opt => opt.MapFrom(src => src.CartHeaderId))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.CartDetails));

            // Маппинг для OrderDetails и CartDetailsDto
            config.CreateMap<OrderDetails, CartDetailsDto>()
                .ForMember(dest => dest.CartDetailsId, opt => opt.MapFrom(src => src.OrderDetailsId))
                .ForMember(dest => dest.CartHeaderId, opt => opt.MapFrom(src => src.OrderHeaderId))
                .ReverseMap()
                .ForMember(dest => dest.OrderDetailsId, opt => opt.MapFrom(src => src.CartDetailsId))
                .ForMember(dest => dest.OrderHeaderId, opt => opt.MapFrom(src => src.CartHeaderId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));
        });

        return mappingConfig;
    }
}