using MediatR;
using NGSBlazor.Shared.Features.Slider.Response;
using NGSBlazor.Shared.Wrapper.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSBlazor.Shared.Features.Slider.Queries
{
    public class GetAllSliderQuery : IRequest<Result<List<SliderResponse>>>
    {
    }
}
