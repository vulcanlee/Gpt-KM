using AutoMapper;
using Backend.AdapterModels;
using Microsoft.AspNetCore.Components.Authorization;

namespace Backend.Services.Interfaces
{
    public interface IRootFileUploadService
    {
        AuthenticationStateProvider AuthenticationStateProvider { get; }
        IMapper Mapper { get; }

        Task<string> CheckWetherCanRootFileUpload(MyUserAdapterModel myUserAdapterModel, string newPassword);
        Task<MyUserAdapterModel> GetCurrentUser();
        Task RootFileUpload(MyUserAdapterModel myUserAdapterModel, string newPassword, string ip);
    }
}