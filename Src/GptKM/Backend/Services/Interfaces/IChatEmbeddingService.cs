using AutoMapper;
using Domains.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Backend.Services.Interfaces
{
    public interface IChatEmbeddingService
    {
        AuthenticationStateProvider AuthenticationStateProvider { get; }
        IMapper Mapper { get; }

        Task<ExpertDirectory> GetDefaultExpertDirectoryAsync(string name);
        Task<ExpertFile> GetExpertFileAsync(string name, ExpertDirectory expertDirectory);
    }
}