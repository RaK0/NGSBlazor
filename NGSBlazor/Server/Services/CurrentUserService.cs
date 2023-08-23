using Infrastructure.Interfaces.Services;
using System.Security.Claims;

namespace NGSBlazor.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; }
        public List<KeyValuePair<string, string>>? Claims { get; set; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            if (Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out Guid result))            
                UserId = result;                    
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
        }
    }
}
