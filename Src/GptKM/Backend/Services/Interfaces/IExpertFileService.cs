using AutoMapper;
using Backend.AdapterModels;
using CommonDomain.DataModels;

namespace Backend.Services.Interfaces
{
    public interface IExpertFileService
    {
        ILogger<ExpertFileService> Logger { get; }
        IMapper Mapper { get; }

        Task<VerifyRecordResult> AddAsync(ExpertFileAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeAddCheckAsync(ExpertFileAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeDeleteCheckAsync(ExpertFileAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeUpdateCheckAsync(ExpertFileAdapterModel paraObject);
        Task<VerifyRecordResult> DeleteAsync(int id);
        Task<DataRequestResult<ExpertFileAdapterModel>> GetAsync(DataRequest dataRequest);
        Task<ExpertFileAdapterModel> GetAsync(int id);
        Task<ExpertFileAdapterModel> GetAsync(string filename);
        Task<DataRequestResult<ExpertFileAdapterModel>> GetByHeaderIDAsync(int id, DataRequest dataRequest);
        Task<VerifyRecordResult> UpdateAsync(ExpertFileAdapterModel paraObject);
    }
}