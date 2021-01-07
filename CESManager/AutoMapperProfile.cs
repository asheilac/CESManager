using AutoMapper;
using CESManager.Dtos.Session;
using CESManager.Models;

namespace CESManager
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Session, GetSessionDto>();
            CreateMap<AddSessionDto, Session>();
        }
    }
}