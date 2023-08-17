using Domain.Entities.Interfaces;
using Infrastructure.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Slider : AEntity<Guid>, IAuditableEntity
    {
        public DateTime ShowOn { get; set; }
        public DateTime HideOn { get; set; }
        public string? TextOn { get; set; }
        public string? Description { get; set; }
        public string? UrlOnClick { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public Slider() { }
    }
}
