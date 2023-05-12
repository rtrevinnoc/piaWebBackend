using AutoMapper;
using Monitores.Entidades;
using Monitores.Recursos;

namespace WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<UserSign, User>()
            //     .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<UserSign, User>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => Guid.NewGuid()));

            CreateMap<User, UserResource>();

            // CreateMap<Auth0User, Auth0UserResource>()
            //     .ForMember(x => x.DefaultPicture, opt => opt.MapFrom(x => x.Picture))
            //     .ForMember(x => x.Picture, opt => opt.MapFrom
            //     (src => string.IsNullOrEmpty(src.UserMetadata.ProfilePicture)
            //         ? src.Picture : src.UserMetadata.ProfilePicture));
        }
    }
}
