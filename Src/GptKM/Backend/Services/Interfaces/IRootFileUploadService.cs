using AutoMapper;
using Domains.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Backend.Services.Interfaces
{
    public interface IRootFileUploadService
    {
        AuthenticationStateProvider AuthenticationStateProvider { get; }
        IMapper Mapper { get; }

        Task<ExpertDirectory> GetDefaultExpertDirectoryAsync(string name);
        Task<ExpertFile> GetExpertFileAndResetStatusAsync(string name, ExpertDirectory expertDirectory);
    }
}