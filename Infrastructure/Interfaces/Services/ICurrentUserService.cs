using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        List<KeyValuePair<string, string>>? Claims { get; }
    }
}
