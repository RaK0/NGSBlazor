using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.DTOModels.Identities.Response
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; } 
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public byte[]? Avatar { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
