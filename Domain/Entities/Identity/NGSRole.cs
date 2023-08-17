using Domain.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class NGSRole : IdentityRole<Guid>, IAuditableEntity
    {        
        public string? Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<NGSRoleClaim> RoleClaims { get; set; }

        public NGSRole() : base()
        {
            RoleClaims = new HashSet<NGSRoleClaim>();
        }
        public NGSRole(string roleName, string? roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<NGSRoleClaim>();
            Description = roleDescription;
        }
    }
}
