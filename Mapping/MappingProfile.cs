using AutoMapper;
using Tienda.Entidades;
using Tienda.Recursos;

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

            CreateMap<UserLogIn, UserSign>();

            CreateMap<ProductResourceIn, Product>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => Guid.NewGuid()))
                .ForMember(x => x.Image, opt => opt.MapFrom(x => ConvertFileToBytes(x.Image)));

            CreateMap<ProductResourceUpdate, ProductUpdate>()
                .ForMember(x => x.Image, opt => opt.MapFrom(x => ConvertFileToBytes(x.Image)));

            CreateMap<Product, ProductResourceOut>()
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => string.Format("https://localhost:7142/api/product/image?name={0}", x.Name)));
        }

        private byte[] ConvertFileToBytes(IFormFile file) {
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                bytes = ms.ToArray();
            }

            return bytes;
        }
    }
}
