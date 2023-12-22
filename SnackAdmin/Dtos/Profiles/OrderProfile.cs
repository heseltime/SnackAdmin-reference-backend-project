using AutoMapper;
using SnackAdmin.Domain;

namespace SnackAdmin.Dtos.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Menu, MenuDto>().ReverseMap();

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(dest => dest.Address, opt => opt.Ignore());



            CreateMap<OrderItem, OrderItemDto>()
                .ForPath(dest => dest.Menu.Id, opt => opt.MapFrom(source => source.MenuId));

            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.MenuId, opt => opt.MapFrom(src => src.Menu.Id));



            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.Address.Id))
                .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.Restaurant.Id))
                .ForMember(dest => dest.OrderedBy, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.GpsLat, opt => opt.MapFrom(src => src.GpsLat))
                .ForMember(dest => dest.GpsLong, opt => opt.MapFrom(src => src.GpsLong))
                .ForMember(dest => dest.FreeText, opt => opt.MapFrom(src => src.FreeText))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));


            CreateMap<DeliveryCondition, DeliveryConditionDto>();
        }
    }
}
