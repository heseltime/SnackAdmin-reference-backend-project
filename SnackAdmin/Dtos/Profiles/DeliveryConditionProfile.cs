using AutoMapper;
using SnackAdmin.Domain;
using SnackAdmin.Dtos;

namespace SnackAdmin.Dtos.Profiles
{
    public class DeliveryConditionProfile : Profile
    {
        public DeliveryConditionProfile()
        {
            CreateMap<DeliveryCondition, DeliveryConditionDto>();
            CreateMap<DeliveryConditionDto, DeliveryCondition>()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore()); ; // Reverse mapping needed here too
        }
    }
}
