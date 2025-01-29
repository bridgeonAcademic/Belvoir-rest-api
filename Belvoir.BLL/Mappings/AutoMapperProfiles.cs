using AutoMapper;
using Belvoir.Bll.DTO;
using Belvoir.Bll.DTO.Delivery;
using Belvoir.Bll.DTO.Design;


//using Belvoir.Controllers.Rentals;
using Belvoir.Bll.DTO.Rental;
using Belvoir.Bll.DTO.Tailor;
using Belvoir.Bll.DTO.User;
using Belvoir.DAL.Models;

namespace Belvoir.Bll.Mappings
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
            
            CreateMap<Tailor, TailorDTO>().ReverseMap();

            CreateMap<User, RegisterResponseDTO>();
            CreateMap<User, TailorResponseDTO>();
            CreateMap<TailorGetDTO, Tailor>().ReverseMap();
            CreateMap<DeliveryDTO, Delivery>().ReverseMap();

            CreateMap<Delivery, DeliveryResponseDTO>();
            CreateMap<RentalSetDTO, RentalProduct>().ForMember(dest=>dest.Id,opt=>opt.Ignore());

            CreateMap<RentalSetDTO, RentalProduct>().ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();

            CreateMap<RentalProduct, RentalViewDTO>().ForMember(dest=>dest.images,opt=>opt.Ignore());


            CreateMap<Design, DesignDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

            CreateMap<Image, ImageDTO>();

            CreateMap<RentalWhishlist, RentalWhishListviewDTO>();

        }
    }
}
