using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NGSBlazor.Shared.Constants.Permission;
using NGSBlazor.Shared.Features.Slider.Commands;
using NGSBlazor.Shared.Features.Slider.Queries;

namespace NGSBlazor.Server.Controllers.v1
{
    public class SliderController : BaseApiVersionController<Slider>
    {
        [Authorize(Policy = Permissions.Slider.Add)]
        [HttpPost]
        public async Task<IActionResult> Post(AddSliderCommand addSlider)
        {
            return Ok(await _mediator.Send(addSlider));
        }

        [HttpPatch] 
        public async Task<IActionResult> Update(EditSliderCommand editSlider) 
        {
            return Ok(await _mediator.Send(editSlider));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetAllSliderQuery()));
        }
    }
}
