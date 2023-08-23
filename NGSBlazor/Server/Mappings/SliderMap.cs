using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using NGSBlazor.Shared.Features.Slider.Commands;
using NGSBlazor.Shared.Features.Slider.Response;

namespace NGSBlazor.Server.Mappings
{
    internal class SliderMap : Profile
    {
        public SliderMap()
        {
            
            CreateMap<AddSliderCommand, Slider>();
            CreateMap<Slider, SliderResponse>();
        }
    }
}
