using AutoMapper;
using Microsoft.Extensions.Localization;
using NGSBlazor.Server.Interfaces.Repositories;

namespace NGSBlazor.Server.Handlers
{
    internal abstract class BaseHandler<TKey, LocalizeSource> where LocalizeSource : class
    {
        protected readonly IMapper _mapper;
        protected readonly IStringLocalizer<LocalizeSource> _localizer;
        protected readonly IUnitOfWork<TKey> _unitOfWork;

        protected BaseHandler(IMapper mapper, IStringLocalizer<LocalizeSource> localizer, IUnitOfWork<TKey> unitOfWork)
        {
            _mapper = mapper;
            _localizer = localizer;
            _unitOfWork = unitOfWork;
        }
    }
}