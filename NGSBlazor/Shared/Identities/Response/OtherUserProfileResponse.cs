using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Identities.Response
{
    public class OtherUserProfileResponse
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public string? Description { get; set; }
        public byte[]? Avatar { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
