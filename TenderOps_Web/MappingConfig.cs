using AutoMapper;
using TenderOps_Web.Models.Dto;


namespace TenderOps_Web
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Mapping for Tender
            CreateMap<TenderCreateDTO, TenderDTO>().ReverseMap(); // من إنشاء إلى العرض فقط
            CreateMap<TenderDTO, TenderUpdateDTO>().ReverseMap();

            // Invoice
            CreateMap<InvoiceCreateDTO, InvoiceDTO>().ReverseMap(); // من إنشاء إلى عرض فقط

            // Tender Category
            CreateMap<TenderCategoryCreateDTO, TenderCategoryDTO>().ReverseMap(); // من إنشاء إلى عرض فقط
            CreateMap<TenderCategoryDTO, TenderCategoryUpdateDTO>().ReverseMap();

            // Partner
            CreateMap<PartnerCreateDTO, PartnerDTO>().ReverseMap();
            CreateMap<PartnerDTO, PartnerUpdateDTO>().ReverseMap();
        }
    }

}

