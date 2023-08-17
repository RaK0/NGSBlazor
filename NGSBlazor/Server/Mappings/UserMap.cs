using AutoMapper;
using Domain.Entities.Identity;
using NGSBlazor.Shared.DTOModels.Identities.Response;

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
