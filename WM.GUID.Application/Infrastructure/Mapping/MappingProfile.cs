using AutoMapper;
using WM.GUID.Application.Queries.ReadGUID;
using WM.GUID.Domain;

namespace WM.GUID.Application.Infrastructure.Mapping
{ 
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GuidMetadata, GuidDTO>();
        }
    }  
}
