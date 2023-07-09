using AutoMapper;
using Backend.AdapterModels;
using CommonDomain.DataModels;

namespace Backend.Services.Interfaces
{
    public interface IExpertDirectoryService
    {
        ILogger<ExpertDirectoryService> Logger { get; }
        IMapper Mapper { get; }

        Task<VerifyRecordResult> AddAsync(ExpertDirectoryAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeAddCheckAsync(ExpertDirectoryAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeDeleteCheckAsync(ExpertDirectoryAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeUpdateCheckAsync(ExpertDirectoryAdapterModel paraObject);
        Task<VerifyRecordResult> DeleteAsync(int id);
        Task<DataRequestResult<ExpertDirectoryAdapterModel>> GetAsync(DataRequest dataRequest);
        Task<ExpertDirectoryAdapterModel> GetAsync(int id);
        Task<VerifyRecordResult> UpdateAsync(ExpertDirectoryAdapterModel paraObject);
    }
}