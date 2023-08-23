using MediatR;
using NGSBlazor.Shared.Wrapper.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Features.Slider.Commands
{
    public class AddSliderCommand : IRequest<Result<bool>>
    {
        public DateTime ShowOn { get; set; }
        public DateTime HideOn { get; set; }
        public string? TextOn { get; set; }
        public string? Description { get; set; }
        public string? UrlOnClick { get; set; }
    }
}
