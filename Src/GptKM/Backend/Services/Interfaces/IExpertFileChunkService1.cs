using AutoMapper;
using Backend.AdapterModels;
using CommonDomain.DataModels;

namespace Backend.Services.Interfaces
{
    public interface IExpertFileChunkService1
    {
        ILogger<ExpertFileChunkService> Logger { get; }
        IMapper Mapper { get; }

        Task<VerifyRecordResult> AddAsync(ExpertFileChunkAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeAddCheckAsync(ExpertFileChunkAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeDeleteCheckAsync(ExpertFileChunkAdapterModel paraObject);
        Task<VerifyRecordResult> BeforeUpdateCheckAsync(ExpertFileChunkAdapterModel paraObject);
        Task<VerifyRecordResult> DeleteAsync(int id);
        Task<DataRequestResult<ExpertFileChunkAdapterModel>> GetAsync(DataRequest dataRequest);
        Task<ExpertFileChunkAdapterModel> GetAsync(int id);
        Task<DataRequestResult<ExpertFileChunkAdapterModel>> GetByHeaderIDAsync(int id, DataRequest dataRequest);
        Task<VerifyRecordResult> UpdateAsync(ExpertFileChunkAdapterModel paraObject);
    }
}