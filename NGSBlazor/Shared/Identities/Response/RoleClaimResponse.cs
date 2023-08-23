using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Identities.Response
{
    public class RoleClaimResponse
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
        public string? Group { get; set; }
        public bool Selected { get; set; }
    }
}
