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

    public class ExpertDirectoryService : IExpertDirectoryService
    {
        #region 欄位與屬性
        private readonly BackendDBContext context;
        public IMapper Mapper { get; }
        public ILogger<ExpertDirectoryService> Logger { get; }
        #endregion

        #region 建構式
        public ExpertDirectoryService(BackendDBContext context, IMapper mapper,
            ILogger<ExpertDirectoryService> logger)
        {
            this.context = context;
            Mapper = mapper;
            Logger = logger;
        }
        #endregion

        #region CRUD 服務
        public async Task<DataRequestResult<ExpertDirectoryAdapterModel>> GetAsync(DataRequest dataRequest)
        {
            List<ExpertDirectoryAdapterModel> data = new();
            DataRequestResult<ExpertDirectoryAdapterModel> result = new();
            var DataSource = context.ExpertDirectory
                .AsNoTracking();

            #region 進行搜尋動作
            if (!string.IsNullOrWhiteSpace(dataRequest.Search))
            {
                DataSource = DataSource
                .Where(x => x.Name.Contains(dataRequest.Search));
            }
            #endregion

            #region 進行排序動作
            if (dataRequest.Sorted != null)
            {
                SortCondition CurrentSortCondition = dataRequest.Sorted;
                switch (CurrentSortCondition.Id)
                {
                    case (int)ExpertDirectorySortEnum.NameDescending:
                        DataSource = DataSource.OrderByDescending(x => x.Name);
                        break;
                    case (int)ExpertDirectorySortEnum.NameAscending:
                        DataSource = DataSource.OrderBy(x => x.Name);
                        break;
                    default:
                        DataSource = DataSource.OrderBy(x => x.Id);
                        break;
                }
            }
            #endregion

            #region 進行分頁
            // 取得記錄總數量，將要用於分頁元件面板使用
            result.Count = DataSource.Cast<ExpertDirectory>().Count();
            DataSource = DataSource.Skip(dataRequest.Skip);
            if (dataRequest.Take != 0)
            {
                DataSource = DataSource.Take(dataRequest.Take);
            }
            #endregion

            #region 在這裡進行取得資料與與額外屬性初始化
            List<ExpertDirectoryAdapterModel> adapterModelObjects =
                Mapper.Map<List<ExpertDirectoryAdapterModel>>(DataSource);

            foreach (var adapterModelItem in adapterModelObjects)
            {
                await OhterDependencyData(adapterModelItem);
            }
            #endregion

            result.Result = adapterModelObjects;
            await Task.Yield();
            return result;
        }

        public async Task<ExpertDirectoryAdapterModel> GetAsync(int id)
        {
            ExpertDirectory item = await context.ExpertDirectory
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            ExpertDirectoryAdapterModel result = Mapper.Map<ExpertDirectoryAdapterModel>(item);
            await OhterDependencyData(result);
            return result;
        }

        public async Task<VerifyRecordResult> AddAsync(ExpertDirectoryAdapterModel paraObject)
        {
            try
            {
                CleanTrackingHelper.Clean<ExpertDirectory>(context);
                ExpertDirectory itemParameter = Mapper.Map<ExpertDirectory>(paraObject);
                CleanTrackingHelper.Clean<ExpertDirectory>(context);
                await context.ExpertDirectory
                    .AddAsync(itemParameter);
                await context.SaveChangesAsync();
                CleanTrackingHelper.Clean<ExpertDirectory>(context);
                return VerifyRecordResultFactory.Build(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "新增記錄發生例外異常");
                return VerifyRecordResultFactory.Build(false, "新增記錄發生例外異常", ex);
            }
        }

        public async Task<VerifyRecordResult> UpdateAsync(ExpertDirectoryAdapterModel paraObject)
        {
            try
            {
                ExpertDirectory itemData = Mapper.Map<ExpertDirectory>(paraObject);
                CleanTrackingHelper.Clean<ExpertDirectory>(context);
                ExpertDirectory item = await context.ExpertDirectory
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
                if (item == null)
                {
                    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.無法修改紀錄);
                }
                else
                {
                    CleanTrackingHelper.Clean<ExpertDirectory>(context);
                    context.Entry(itemData).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    CleanTrackingHelper.Clean<ExpertDirectory>(context);
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
                CleanTrackingHelper.Clean<ExpertDirectory>(context);
                ExpertDirectory item = await context.ExpertDirectory
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                {
                    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.無法刪除紀錄);
                }
                else
                {
                    CleanTrackingHelper.Clean<ExpertDirectory>(context);
                    context.Entry(item).State = EntityState.Deleted;
                    await context.SaveChangesAsync();
                    CleanTrackingHelper.Clean<ExpertDirectory>(context);
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
        public async Task<VerifyRecordResult> BeforeAddCheckAsync(ExpertDirectoryAdapterModel paraObject)
        {
            var searchItem = await context.ExpertDirectory
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == paraObject.Name);
            if (searchItem != null)
            {
                return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.要新增的紀錄已經存在無法新增);
            }
            return VerifyRecordResultFactory.Build(true);
        }

        public async Task<VerifyRecordResult> BeforeUpdateCheckAsync(ExpertDirectoryAdapterModel paraObject)
        {
            CleanTrackingHelper.Clean<ExpertDirectory>(context);
            var searchItem = await context.ExpertDirectory
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
            if (searchItem == null)
            {
                return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.要更新的紀錄_發生同時存取衝突_已經不存在資料庫上);
            }

            searchItem = await context.ExpertDirectory
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Name == paraObject.Name &&
               x.Id != paraObject.Id);
            if (searchItem != null)
            {
                return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.要修改的紀錄已經存在無法修改);
            }
            return VerifyRecordResultFactory.Build(true);
        }

        public async Task<VerifyRecordResult> BeforeDeleteCheckAsync(ExpertDirectoryAdapterModel paraObject)
        {
            try
            {
                CleanTrackingHelper.Clean<OrderItem>(context);
                CleanTrackingHelper.Clean<ExpertDirectory>(context);

                var searchItem = await context.ExpertDirectory
                 .AsNoTracking()
                 .FirstOrDefaultAsync(x => x.Id == paraObject.Id);
                if (searchItem == null)
                {
                    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.無法刪除紀錄_要刪除的紀錄已經不存在資料庫上);
                }

                var searchOrderItemItem = await context.ExpertFile
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ExpertDirectoryId == paraObject.Id);
                if (searchOrderItemItem != null)
                {
                    return VerifyRecordResultFactory.Build(false, ErrorMessageEnum.該紀錄無法刪除因為有其他資料表在使用中);
                }
                return VerifyRecordResultFactory.Build(true);
            }
            catch (Exception ex)
            {
                return VerifyRecordResultFactory.Build(false, "刪除記錄發生例外異常", ex);
            }
        }
        #endregion

        #region 其他服務方法
        Task OhterDependencyData(ExpertDirectoryAdapterModel data)
        {
            return Task.FromResult(0);
        }
        #endregion
    }
}
