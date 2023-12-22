using AutoMapper;
using SnackAdmin.Domain;

namespace SnackAdmin.Dtos.Profiles
{
    public class RestaurantProfile : Profile
    {
        public RestaurantProfile()
        {
            CreateMap<Address, AddressDto>();

            CreateMap<DeliveryCondition, DeliveryConditionDto>();

            CreateMap<OpeningHour, OpeningHourDtoForToday>();

            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryCondition, opt => opt.Ignore())
                .ForMember(dest => dest.OpeningHour, opt => opt.Ignore());
        }
    }
}
