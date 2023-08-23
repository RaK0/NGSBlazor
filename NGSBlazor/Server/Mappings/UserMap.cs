using AutoMapper;
using Domain.Entities.Identity;
using NGSBlazor.Shared.Identities.Response;

namespace NGSBlazor.Server.Mappings
{
    internal class UserMap:Profile
    {
        public UserMap() 
        {
            CreateMap<NGSUser, UserResponse>();
            CreateMap<NGSUser, OtherUserProfileResponse>();
        }
    }
}
