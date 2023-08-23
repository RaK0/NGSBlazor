using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using NGSBlazor.Server.Interfaces.Repositories;
using NGSBlazor.Shared.Features.Slider.Commands;
using NGSBlazor.Shared.Wrapper.Result;

namespace NGSBlazor.Server.Handlers.SliderHandlers
{
    internal class EditSliderHandler : BaseHandler<Guid, EditSliderHandler>, IRequestHandler<EditSliderCommand, Result>
    {
        public EditSliderHandler(IMapper mapper, IStringLocalizer<EditSliderHandler> localizer, IUnitOfWork<Guid> unitOfWork) : base(mapper, localizer, unitOfWork)
        {
        }

        public async Task<Result> Handle(EditSliderCommand request, CancellationToken cancellationToken)
        {
            Slider? slider = await _unitOfWork.Repository<Slider>().GetByIdAsync(request.Id);
            if (slider is null)
                return (Result)Result.Fail(_localizer["Cant find"]);

            slider.TextOn = request.TextOn ?? slider.TextOn;
            slider.UrlOnClick = request.UrlOnClick ?? slider.UrlOnClick;
            slider.Description = request.Description ?? slider.Description;
            slider.ShowOn = request.ShowOn;
            slider.HideOn = request.HideOn;

            return (Result)Result.Success(_localizer["Success"]);
        }
    }
}
