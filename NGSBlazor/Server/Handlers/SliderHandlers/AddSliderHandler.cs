using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using NGSBlazor.Server.Interfaces.Repositories;
using NGSBlazor.Shared.Features.Slider.Commands;
using NGSBlazor.Shared.Wrapper.Result;

namespace NGSBlazor.Server.Handlers.SliderHandlers
{
    internal class AddSliderHandler : BaseHandler<Guid, AddSliderHandler>,IRequestHandler<AddSliderCommand, Result<bool>>
    {
        public AddSliderHandler(IMapper mapper, IStringLocalizer<AddSliderHandler> localizer, IUnitOfWork<Guid> unitOfWork) : base(mapper, localizer, unitOfWork)
        {
        }

        public async Task<Result<bool>> Handle(AddSliderCommand request, CancellationToken cancellationToken)
        {
            Slider slider = _mapper.Map<Slider>(request);
            await _unitOfWork.Repository<Slider>().AddAsync(slider);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true, _localizer["Success"]);
        }
    }
}
