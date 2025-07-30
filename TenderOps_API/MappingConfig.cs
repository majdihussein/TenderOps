using AutoMapper;
using TenderOps_API.Models;
using TenderOps_API.Models.Dto;

namespace TenderOps_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<TenderCreateDTO, Tender>().ReverseMap();

            CreateMap<Tender, TenderUpdateDTO>().ReverseMap();
            CreateMap<Tender, TenderDTO>()
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Email : null))
                .ForMember(dest => dest.TenderCategoryName, opt => opt.MapFrom(src => src.TenderCategory.Name))
                .ForMember(dest => dest.PartnerName, opt => opt.MapFrom(src => src.Partner != null ? src.Partner.Name : null))
                .ReverseMap();


            CreateMap<Invoice, InvoiceCreateDTO>().ReverseMap();
            CreateMap<Invoice, InvoiceDTO>()
            .ForMember(dest => dest.TenderName, opt => opt.MapFrom(src => src.Tender.TenderName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))  // هذا اسم المستخدم اللي أنشأ الفاتورة
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User != null ? src.User.Id : null))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null));


            CreateMap<TenderCategory, TenderCategoryDTO>().ReverseMap();
            CreateMap<TenderCategory, TenderCategoryCreateDTO>().ReverseMap();
            CreateMap<TenderCategory, TenderCategoryUpdateDTO>().ReverseMap();

            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.PartnerId, opt => opt
                .MapFrom(src => src.PartnerId))
                .ReverseMap();

            CreateMap<Partner, PartnerDTO>().ReverseMap();
            CreateMap<Partner, PartnerCreateDTO>().ReverseMap();
            CreateMap<Partner, PartnerUpdateDTO>().ReverseMap();



        }
    }
}
