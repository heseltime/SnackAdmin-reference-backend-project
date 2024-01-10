using AutoMapper;
using SnackAdmin.Domain;

namespace SnackAdmin.Dtos.Profiles
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<Menu, MenuDto>().ReverseMap();
        }
    }
}
