using AutoMapper;
using Belvoir.Controllers.Rentals;
using Belvoir.DTO.Rental;
using Belvoir.DTO.User;
using Belvoir.Models;

namespace Belvoir.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "User"))
                .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => false));

            CreateMap<User, RegisterResponseDTO>();

            CreateMap<RentalSetDTO, RentalProduct>().ForMember(dest=>dest.Id,opt=>opt.Ignore());

            CreateMap<RentalSetDTO, RentalProduct>().ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();

            CreateMap<RentalProduct, RentalViewDTO>().ForMember(dest=>dest.images,opt=>opt.Ignore());
        }
    }
}
