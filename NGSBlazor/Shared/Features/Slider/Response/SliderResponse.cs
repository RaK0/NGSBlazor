using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Features.Slider.Response
{
    public class SliderResponse
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
    }
}
