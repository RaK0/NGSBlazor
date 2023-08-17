using Domain.Entities.Interfaces;
using Infrastructure.Interfaces.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class NGSUser : IdentityUser<Guid>, IAuditableEntity
    {
        public string? Description { get; set; }
        public bool Deleted { get; set; } = false;
        public virtual byte[]? Avatar { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid LastModifiedBy { get;set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public NGSUser()
        {

        }
    }
}
