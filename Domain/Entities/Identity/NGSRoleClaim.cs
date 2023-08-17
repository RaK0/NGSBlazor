using Domain.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class NGSRoleClaim : IdentityRoleClaim<Guid>, IAuditableEntity
    {
        public string? Description { get; set; }
        public string? Group { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual NGSRole Role { get; set; }

        public NGSRoleClaim() : base() { }
        public NGSRoleClaim(string? roleClaimDescription = null, string? roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}
