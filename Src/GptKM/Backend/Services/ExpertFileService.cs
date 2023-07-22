using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    using AutoMapper;
    using Backend.AdapterModels;
    using Backend.SortModels;
    using Domains.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using BAL.Factories;
    using BAL.Helpers;
    using CommonDomain.DataModels;
    using CommonDomain.Enums;
    using System;
    using Backend.Services.Interfaces;

    public class ExpertFileService : IExpertFileService
    {
        #region 欄位與屬性
        private readonly BackendDBContext context;
        public IMapper Mapper { get; }
        public ILogger<ExpertFileService> Logger { get; }
        #endregion

        #region 建構式
        public ExpertFileService(BackendDBContext context, IMapper mapper,
            ILogger<ExpertFileService> logger)
        {
            this.context = context;
            Mapper = mapper;
            Logger = logger;
        }
        #endregion

        #region CRUD 服務
        public async Task<DataRequestResult<ExpertFileAdapterModel>> GetAsync(DataRequest dataRequest)
        {
            List<ExpertFileAdapterModel> data = new();
            DataRequestResult<ExpertFileAdapterModel> result = new();
            var DataSource = context.ExpertFile
                .AsNoTracking()
                .Include(x => x.ExpertDirectory)
                .Include(x => x.ExpertFileChunk)
                .AsQueryable();

            #region 進行搜尋動作
            if (!string.IsNullOrWhiteSpace(dataRequest.Search))
            {
                DataSource = DataSource
                .Where(x => x.FileName.Contains(dataRequest.Search));
            }
            #endregion

            #region 進行排序動作
            if (dataRequest.Sorted != null)
            {
                SortCondition CurrentSortCondition = dataRequest.Sorted;
                switch (CurrentSortCondition.Id)
                {
                    case (int)ExpertFileSortEnum.NameDescending:
                        DataSource = DataSource.OrderByDescending(x => x.FileName);
                        break;
                    case (int)ExpertFileSortEnum.NameAscending:
                        DataSource = DataSource.OrderBy(x => x.FileName);
                        break;
                    default:
                        DataSource = DataSource.OrderBy(x => x.Id);
                        break;
                }
            }
            #endregion

            #region 進行分頁
            // 取得記錄總數量，將要用於分頁元件面板使用
            result.Count = DataSource.Cast<ExpertFile>().Count();
            DataSource = DataSource.Skip(dataRequest.Skip);
            if (dataRequest.Take != 0)
            {
                DataSource = DataSource.Take(dataRequest.Take);
            }
            #endregion

            #region 在這裡進行取得資料與與額外屬性初始化
            List<ExpertFileAdapterModel> adapterModelObjects =
                Mapper.Map<List<ExpertFileAdapterModel>>(DataSource);

            foreach (var adapterModelItem in adapterModelObjects)
            {
                await OhterDependencyData(adapterModelItem);

            }
            #endregion

            result.Result = adapterModelObjects;
            await Task.Yield();
            return result;
        }

        public async Task<DataRequestResult<ExpertFileAdapterModel>> GetByHeaderIDAsync(int id, DataRequest dataRequest)
        {
            List<ExpertFileAdapterModel> data = new();
            DataRequestResult<ExpertFileAdapterModel> result = new();
            var DataSource = context.ExpertFile
                .AsNoTracking()
                .Include(x => x.ExpertDirectory)
                .Include(x => x.ExpertFileChunk)
                .Where(x => x.ExpertDirectoryId == id);

            #region 進行搜尋動作
            if (!string.IsNullOrWhiteSpace(dataRequest.Search))
            {
                DataSource = DataSource
                .Where(x => x.FileName.Contains(dataRequest.Search));
            }
            #endregion

            #region 進行排序動作
            if (dataRequest.Sorted != null)
            {
                SortCondition CurrentSortCondition = dataRequest.Sorted;
                switch (CurrentSortCondition.Id)
                {
                    case (int)ExpertFileSortEnum.NameDescending:
                        DataSource = DataSource.OrderByDescending(x => x.FileName);
                        break;
                    case (int)ExpertFileSortEnum.NameAscending:
                        DataSource = DataSource.OrderBy(x => x.FileName);
                        break;
                    default:
                        DataSource = DataSource.OrderBy(x => x.FileName);
                        break;
                }
            }
            #endregion

            #region 進行分頁
            // 取得記錄總數量，將要用於分頁元件面板使用
            result.Count = DataSource.Cast<ExpertFile>().Count();
            DataSource = DataSource.Skip(dataRequest.Skip);
            if (dataRequest.Take != 0)
            {
                DataSource = DataSource.Take(dataRequest.Take);
            }
            #endregion

            #region 在這裡進行取得資料與與額外屬性初始化
            List<ExpertFileAdapterModel> adapterModelObjects =
                Mapper.Map<List<ExpertFileAdapterModel>>(DataSource);

            foreach (var adapterModelItem in adapterModelObjects)
            {
                await OhterDependencyData(adapterModelItem);
            }
            #endregion

            result.Result = adapterModelObjects;
            await Task.Yield();
            return result;
        }

        public async Task<ExpertFileAdapterModel> GetAsync(int id)
        {
            ExpertFile item = await context.ExpertFile
                .AsNoTracking()
                .Include(x => x.ExpertFileChunk)
                .Include(x => x.ExpertDirectory)
                .FirstOrDefaultAsync(x => x.Id == id);
            ExpertFileAdapterModel result = Mapper.Map<ExpertFileAdapterModel>(item);
            await OhterDependencyData(result);
            return result;
        }

        public async Task<VerifyRecordResult> AddAsync(ExpertFileAdapterModel paraObject)
        {
            try
            {
                ExpertFile itemParameter = Mapper.Map<ExpertFile>(paraObject);
                CleanTrackingHelper.Clean<ExpertFile>(context);
                await context.ExpertFile
                    .AddAsync(itemParameter);
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<ExpertFile>(context);
                return VerifyRecordResultFactory.Build(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "新增記錄發生例外異常");
                return VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex);
            }
        }

        public async Task<VerifyRecordResult> UpdateAsync(ExpertFileAdapterModel paraObject)
        {
            try
            {
                ExpertFile itemData = Mapper.Map<ExpertFile>(paraObject);
                CleanTrackingHelper.Clean<ExpertFile>(context);
                ExpertFile item = await context.ExpertFile
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
                if (item == null)
                {
                    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.無法修改紀錄);
                }
                else
                {
                    CleanTrackingHelper.Clean<ExpertFile>(context);
                    context.Entry(itemData).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    CleanTrackingHelper.Clean<ExpertFile>(context);
                    return VerifyRecordResultFactory.Build(true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "修改記錄發生例外異常");
                return VerifyRecordResultFactory.Build(false, "修改記錄發生例外異常", ex);
            }
        }

        public async Task<VerifyRecordResult> DeleteAsync(int id)
        {
            try
            {
                CleanTrackingHelper.Clean<ExpertFile>(context);
                CleanTrackingHelper.Clean<ExpertFileChunk>(context);
                ExpertFile item = await context.ExpertFile
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                {
                    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.無法刪除紀錄);
                }
                else
                {
                    CleanTrackingHelper.Clean<ExpertFile>(context);
                    CleanTrackingHelper.Clean<ExpertFileChunk>(context);

                    #region 刪除 Chunk
                    List<ExpertFileChunk> fileChunks = await context.ExpertFileChunk
                        .Where(x => x.ExpertFileId == id)
                        .ToListAsync();
                    context.ExpertFileChunk.RemoveRange(fileChunks);
                    await context.SaveChangesAsync();
                    #endregion

                    context.Entry(item).State = EntityState.Deleted;
                    await context.SaveChangesAsync();
                    CleanTrackingHelper.Clean<ExpertFile>(context);
                    CleanTrackingHelper.Clean<ExpertFileChunk>(context);
                    return VerifyRecordResultFactory.Build(true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "刪除記錄發生例外異常");
                return VerifyRecordResultFactory.Build(false, "刪除記錄發生例外異常", ex);
            }
        }
        #endregion

        #region CRUD 的限制條件檢查
        public async Task<VerifyRecordResult> BeforeAddCheckAsync(ExpertFileAdapterModel paraObject)
        {
            CleanTrackingHelper.Clean<ExpertFile>(context);
            if (string.IsNullOrEmpty(paraObject.FileName))
            {
                return VerifyRecordResultFactory.Build(false, "檔案名稱不可為空白");
            }

            var item = await context.ExpertFile
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ExpertDirectoryId == paraObject.ExpertDirectoryId &&
                x.FileName == paraObject.FileName);
            if (item != null)
            {
                return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.該訂單已經存在該產品_不能重複同樣的商品在一訂單內);
            }
            return VerifyRecordResultFactory.Build(true);
        }

        public async Task<VerifyRecordResult> BeforeUpdateCheckAsync(ExpertFileAdapterModel paraObject)
        {
            CleanTrackingHelper.Clean<ExpertFile>(context);
            if (string.IsNullOrEmpty(paraObject.FileName))
            {
                return VerifyRecordResultFactory.Build(false, "檔案名稱不可為空白");
            }

            var searchItem = await context.ExpertFile
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (searchItem == null)
            {
                return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.要更新的紀錄_發生同時存取衝突_已經不存在資料庫上);
            }

            searchItem = await context.ExpertFile
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ExpertDirectoryId == paraObject.ExpertDirectoryId &&
                x.FileName == paraObject.FileName &&
                x.Id != paraObject.Id);
            if (searchItem != null)
            {
                return VerifyRecordResultFactory.Build(false, "該檔案已經存，不能修改");
            }
            return VerifyRecordResultFactory.Build(true);
        }

        public async Task<VerifyRecordResult> BeforeDeleteCheckAsync(ExpertFileAdapterModel paraObject)
        {
            CleanTrackingHelper.Clean<ExpertFile>(context);
            var searchItem = await context.ExpertFile
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (searchItem == null)
            {
                return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.無法刪除紀錄_要刪除的紀錄已經不存在資料庫上);
            }

            return VerifyRecordResultFactory.Build(true);
        }
        #endregion

        #region 其他服務方法
        Task OhterDependencyData(ExpertFileAdapterModel data)
        {
            data.ConvertDirectoryName = data.ExpertDirectory.Name;
            return Task.FromResult(0);
        }
        #endregion
    }
}
