using AutoMapper;
using TeamTrack.Api.Models;
using TeamTrack.Core.Entities;

namespace TeamTrack.Api.Tools
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}
