﻿namespace Backend.Adapters
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using AutoMapper;
    using Backend.AdapterModels;
    using CommonDomain.DataModels;
    using Newtonsoft.Json;
    using Syncfusion.Blazor;
    using Syncfusion.Blazor.Data;
    using Backend.Services.Interfaces;

    public partial class ExpertFileAdapter : DataAdaptor<IExpertFileService>
    {
        [Parameter]
        public ILogger<ExpertFileAdapter> Logger { get; set; }
        [Parameter]
        public int HeaderID { get; set; }

        [Parameter]
        public SortCondition CurrentSortCondition { get; set; }

        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            #region 建立查詢物件
            DataRequest dataRequest = new DataRequest()
            {
                Skip = dataManagerRequest.Skip,
                Take = dataManagerRequest.Take,
            };
            if (dataManagerRequest.Search != null && dataManagerRequest.Search.Count > 0)
            {
                var keyword = dataManagerRequest.Search[0].Key;
                dataRequest.Search = keyword;
            }
            if (CurrentSortCondition != null)
            {
                dataRequest.Sorted = CurrentSortCondition;
            }
            #endregion

            #region 發出查詢要求
            try
            {
                DataRequestResult<ExpertFileAdapterModel> adaptorModelObjects;
                if (HeaderID <= 0)
                {
                    adaptorModelObjects = await Service.GetAsync(dataRequest);
                }
                else
                {
                    adaptorModelObjects = await Service.GetByHeaderIDAsync(HeaderID, dataRequest);
                }
                var item = dataManagerRequest.RequiresCounts
                    ? new DataResult() { Result = adaptorModelObjects.Result, Count = adaptorModelObjects.Count }
                    : (object)adaptorModelObjects.Result;
                await Task.Yield();
                return item;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ExpertFileAdapter 發生例外異常");
                return new DataResult() { Result = new List<ExpertFileAdapterModel>(), Count = 0 };
            }
            #endregion
        }
    }
}

