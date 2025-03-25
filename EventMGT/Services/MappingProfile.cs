using AutoMapper;
using EventMGT.DTOs;
using EventMGT.Models;

namespace EventMGT.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventUser, EventUserDto>();

            CreateMap<RegistrationRequestDto, EventUser>()
                .ForMember(dest => dest.IsRegisteredForMeal, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
