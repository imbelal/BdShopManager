using AutoMapper;
using Domain.Dtos;
using Domain.Entities;

namespace Domain.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
        }
    }
}
