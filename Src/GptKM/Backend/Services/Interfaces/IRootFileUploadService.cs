using AutoMapper;
using Backend.AdapterModels;
using Microsoft.AspNetCore.Components.Authorization;

namespace Backend.Services.Interfaces
{
    public interface IRootFileUploadService
    {
        AuthenticationStateProvider AuthenticationStateProvider { get; }
        IMapper Mapper { get; }

    }
}