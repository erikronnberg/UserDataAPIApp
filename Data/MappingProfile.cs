using AutoMapper;
using UserDataAPIApp.Models;

namespace UserDataAPIApp.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, GetAll>().ReverseMap();
        }
    }
}
