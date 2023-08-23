using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using NGSBlazor.Server.Interfaces.Repositories;
using NGSBlazor.Shared.Features.Slider.Queries;
using NGSBlazor.Shared.Features.Slider.Response;
using NGSBlazor.Shared.Wrapper.Result;

namespace NGSBlazor.Server.Handlers.SliderHandlers
{
    internal class GetAllSliderHandler : BaseHandler<Guid, GetAllSliderHandler>, IRequestHandler<GetAllSliderQuery, Result<List<SliderResponse>>>
    {
        public GetAllSliderHandler(IMapper mapper, IStringLocalizer<GetAllSliderHandler> localizer, IUnitOfWork<Guid> unitOfWork) : base(mapper, localizer, unitOfWork)
        {
        }

        public Task<Result<List<SliderResponse>>> Handle(GetAllSliderQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Slider> sliders = _unitOfWork.Repository<Slider>().Entities;
            List<SliderResponse>response = _mapper.Map<List<SliderResponse>>(sliders);
            return Result<List<SliderResponse>>.SuccessAsync(response);
        }
    }
}
