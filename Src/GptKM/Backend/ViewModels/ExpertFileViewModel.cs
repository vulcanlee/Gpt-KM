﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.ViewModels
{
    using AutoMapper;
    using Backend.AdapterModels;
    using Backend.Helpers;
    using Backend.Interfaces;
    using Backend.SortModels;
    using Domains.Models;
    using Microsoft.AspNetCore.Components.Forms;
    using BAL.Helpers;
    using CommonDomain.DataModels;
    using Syncfusion.Blazor.Grids;
    using Syncfusion.Blazor.Navigations;
    using Backend.Models;
    using Backend.Services.Interfaces;
    using GptLibrary.Services;
    using GptLibrary.Helpers;
    using CommonDomain.Enums;

    public class ExpertFileViewModel
    {
        #region Constructor
        public ExpertFileViewModel(IExpertFileService CurrentService,
           BackendDBContext context, IMapper Mapper,
           TranscationResultHelper transcationResultHelper,
           EmbeddingSearchHelper embeddingSearchHelper, GptExpertFileService gptExpertFileService)
        {
            this.CurrentService = CurrentService;
            this.context = context;
            mapper = Mapper;
            TranscationResultHelper = transcationResultHelper;
            this.embeddingSearchHelper = embeddingSearchHelper;
            this.gptExpertFileService = gptExpertFileService;
            ExpertFileSort.Initialization(SortConditions);

            #region 工具列按鈕初始化
            Toolbaritems.Add(new ItemModel()
            {
                Id = ButtonIdHelper.ButtonIdRefresh,
                Text = "重新整理",
                TooltipText = "重新整理",
                PrefixIcon = "mdi mdi-refresh",
                Align = ItemAlign.Left,
            });
            Toolbaritems.Add("Search");
            #endregion
        }
        #endregion

        #region Property
        /// <summary>
        /// 是否要顯示紀錄新增或修改對話窗 
        /// </summary>
        public bool IsShowEditRecord { get; set; } = false;
        /// <summary>
        /// 是否要顯示關聯多筆資料表的 CRUD 對話窗
        /// </summary>
        public bool IsShowMoreDetailsRecord { get; set; } = false;
        /// <summary>
        /// 現在正在新增或修改的紀錄  
        /// </summary>
        public ExpertFileAdapterModel CurrentRecord { get; set; } = new ExpertFileAdapterModel();
        /// <summary>
        /// 現在正在刪除的紀錄  
        /// </summary>
        public ExpertFileAdapterModel CurrentNeedDeleteRecord { get; set; } = new ExpertFileAdapterModel();
        /// <summary>
        /// 保存與資料編輯程式相關的中繼資料
        /// </summary>
        public EditContext LocalEditContext { get; set; }
        /// <summary>
        /// 是否顯示選取其他清單記錄對話窗 
        /// </summary>
        public bool ShowAontherRecordPicker { get; set; } = false;
        /// <summary>
        /// 父參考物件的 Id 
        /// </summary>
        public MasterRecord Header { get; set; } = new MasterRecord();
        /// <summary>
        /// 可以選擇排序條件清單
        /// </summary>
        public List<SortCondition> SortConditions { get; set; } = new List<SortCondition>();
        /// <summary>
        /// 現在選擇排序條件項目
        /// </summary>
        public SortCondition CurrentSortCondition { get; set; } = new SortCondition();
        /// <summary>
        /// 用於控制、更新明細清單 Grid 
        /// </summary>
        public IDataGrid ShowMoreDetailsGrid { get; set; }
        /// <summary>
        /// 明細清單 Grid 的對話窗主題 
        /// </summary>
        public string ShowMoreDetailsRecordDialogTitle { get; set; } = "";
        /// <summary>
        /// 新增或修改對話窗的標題 
        /// </summary>
        public string EditRecordDialogTitle { get; set; } = "";
        /// <summary>
        /// 指定 Grid 上方可以使用的按鈕項目清單
        /// </summary>
        public List<object> Toolbaritems { get; set; } = new List<object>();

        #region 訊息說明之對話窗使用的變數
        /// <summary>
        /// 確認對話窗設定
        /// </summary>
        public ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
        /// <summary>
        /// 訊息對話窗設定
        /// </summary>
        public MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
        public TranscationResultHelper TranscationResultHelper { get; }
        #endregion
        #endregion

        #region Field
        /// <summary>
        /// 對選取紀錄進行 新增 或者 修改 
        /// </summary>
        bool isNewRecordMode;
        /// <summary>
        /// 當前記錄需要用到的 Service 物件 
        /// </summary>
        private readonly IExpertFileService CurrentService;
        private readonly BackendDBContext context;
        private readonly IMapper mapper;
        private readonly EmbeddingSearchHelper embeddingSearchHelper;
        private readonly GptExpertFileService gptExpertFileService;

        /// <summary>
        /// 這個元件整體的通用介面方法
        /// </summary>
        IRazorPage thisView;
        /// <summary>
        /// 當前 Grid 元件可以使用的通用方法
        /// </summary>
        IDataGrid dataGrid;
        #endregion

        #region Method
        #region DataGrid 初始化
        /// <summary>
        /// 將會於 生命週期事件 OnInitialized / OnAfterRenderAsync 觸發此方法
        /// </summary>
        /// <param name="razorPage">當前元件的物件</param>
        /// <param name="dataGrid">當前 Grid 的元件</param>
        public void Setup(IRazorPage razorPage, IDataGrid dataGrid)
        {
            thisView = razorPage;
            this.dataGrid = dataGrid;
        }
        #endregion

        #region 工具列事件 (新增)
        public void ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == ButtonIdHelper.ButtonIdAdd)
            {
                CurrentRecord = new ExpertFileAdapterModel();
                #region 針對新增的紀錄所要做的初始值設定商業邏輯
                #endregion
                EditRecordDialogTitle = "新增紀錄";
                isNewRecordMode = true;
                IsShowEditRecord = true;
                CurrentRecord.ExpertDirectoryId = Header.Id;
                //CurrentRecord.Name = Header.Title;
            }
            else if (args.Item.Id == ButtonIdHelper.ButtonIdRefresh)
            {
                dataGrid.RefreshGrid();
            }
        }
        #endregion

        #region 記錄列的按鈕事件 (修改與刪除) 
        public async Task OnCommandClicked(CommandClickEventArgs<ExpertFileAdapterModel> args)
        {
            ExpertFileAdapterModel item = args.RowData as ExpertFileAdapterModel;
            if (args.CommandColumn.ButtonOption.IconCss == ButtonIdHelper.ButtonIdEdit)
            {
                #region 點選 修改紀錄 按鈕
                CurrentRecord = item.Clone();
                EditRecordDialogTitle = "修改紀錄";
                IsShowEditRecord = true;
                isNewRecordMode = false;
                #endregion
            }
            else if (args.CommandColumn.ButtonOption.IconCss == ButtonIdHelper.ButtonIdReset)
            {
                #region 點選 重新設定 按鈕
                CurrentNeedDeleteRecord = item;

                #region 重新設定 這筆紀錄
                await Task.Yield();

                var expertFileResult = await gptExpertFileService.GetAsync(CurrentNeedDeleteRecord.Id);
                if (expertFileResult.Status == true)
                {
                    ExpertFile expertFile = expertFileResult.Payload;
                    await embeddingSearchHelper.DeleteAllChunkRawFileAsync(expertFile);

                    try
                    {
                        CleanTrackingHelper.Clean<ExpertDirectory>(context);
                        CleanTrackingHelper.Clean<ExpertFile>(context);
                        CleanTrackingHelper.Clean<ExpertFileChunk>(context);
                        context.ExpertFileChunk.RemoveRange(expertFile.ExpertFileChunk);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync(ex.Message);
                    }

                    CurrentNeedDeleteRecord = await CurrentService.GetAsync(CurrentNeedDeleteRecord.Id);
                    CleanTrackingHelper.Clean<ExpertDirectory>(context);
                    CleanTrackingHelper.Clean<ExpertFile>(context);
                    CleanTrackingHelper.Clean<ExpertFileChunk>(context);
                    CurrentNeedDeleteRecord.ProcessingStatus = ExpertFileStatusEnum.Begin;
                    var verifyRecordResult = await CurrentService.UpdateAsync(CurrentNeedDeleteRecord);
                    dataGrid.RefreshGrid();
                }
                #endregion
                #endregion
            }
            else if (args.CommandColumn.ButtonOption.IconCss == ButtonIdHelper.ButtonIdDelete)
            {
                #region 點選 刪除紀錄 按鈕
                CurrentNeedDeleteRecord = item;

                #region 檢查關聯資料是否存在
                var checkedResult = await CurrentService
                    .BeforeDeleteCheckAsync(CurrentNeedDeleteRecord);
                await Task.Delay(100);
                if (checkedResult.Success == false)
                {
                    MessageBox.Show("400px", "200px", "警告",
                        ErrorMessageMappingHelper.Instance.GetErrorMessage(checkedResult.MessageId),
                        MessageBox.HiddenAsync);
                    await Task.Yield();
                    await thisView.NeedRefreshAsync();
                    return;
                }
                #endregion

                #region 刪除這筆紀錄
                await Task.Yield();
                var checkTask = ConfirmMessageBox.ShowAsync("400px", "200px", "警告",
                     "確認要刪除這筆紀錄嗎?", ConfirmMessageBox.HiddenAsync);
                await thisView.NeedRefreshAsync();
                var checkAgain = await checkTask;
                if (checkAgain == true)
                {
                    var expertFileResult = await gptExpertFileService.GetAsync(CurrentNeedDeleteRecord.Id);
                    if (expertFileResult.Status == true)
                    {
                        ExpertFile expertFile = expertFileResult.Payload;
                        await embeddingSearchHelper.DeleteAllChunkRawFileAsync(expertFile);

                        try
                        {
                            CleanTrackingHelper.Clean<ExpertDirectory>(context);
                            CleanTrackingHelper.Clean<ExpertFile>(context);
                            CleanTrackingHelper.Clean<ExpertFileChunk>(context);
                            context.ExpertFileChunk.RemoveRange(expertFile.ExpertFileChunk);
                            await context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            await Console.Out.WriteLineAsync(ex.Message);
                        }

                        CleanTrackingHelper.Clean<ExpertFile>(context);
                        var verifyRecordResult = await CurrentService.DeleteAsync(CurrentNeedDeleteRecord.Id);
                        await TranscationResultHelper.CheckDatabaseResult(MessageBox, verifyRecordResult);

                        await embeddingSearchHelper.DeleteExpertFileAsync(expertFile);
                        dataGrid.RefreshGrid();
                    }
                }
                #endregion
                #endregion
            }
        }
        #endregion

        #region 修改紀錄對話窗的按鈕事件
        public void OnEditContestChanged(EditContext context)
        {
            LocalEditContext = context;
        }

        public void OnRecordEditCancel()
        {
            IsShowEditRecord = false;
        }

        public async Task OnRecordEditConfirm()
        {
            #region 進行 Form Validation 檢查驗證作業
            if (LocalEditContext.Validate() == false)
            {
                return;
            }
            #endregion

            #region 檢查資料完整性
            if (isNewRecordMode == true)
            {
                var checkedResult = await CurrentService
                    .BeforeAddCheckAsync(CurrentRecord);
                if (checkedResult.Success == false)
                {
                    MessageBox.Show("400px", "200px", "警告",
                        VerifyRecordResultHelper.GetMessageString(checkedResult), MessageBox.HiddenAsync);
                    await thisView.NeedRefreshAsync();
                    return;
                }
            }
            else
            {
                var checkedResult = await CurrentService
                    .BeforeUpdateCheckAsync(CurrentRecord);
                if (checkedResult.Success == false)
                {
                    MessageBox.Show("400px", "200px", "警告",
                        VerifyRecordResultHelper.GetMessageString(checkedResult), MessageBox.HiddenAsync);
                    await thisView.NeedRefreshAsync();
                    return;
                }
            }
            #endregion

            if (IsShowEditRecord == true)
            {
                if (isNewRecordMode == true)
                {
                    var verifyRecordResult = await CurrentService.AddAsync(CurrentRecord);
                    await TranscationResultHelper.CheckDatabaseResult(MessageBox, verifyRecordResult);
                    dataGrid.RefreshGrid();
                }
                else
                {
                    var verifyRecordResult = await CurrentService.UpdateAsync(CurrentRecord);
                    await TranscationResultHelper.CheckDatabaseResult(MessageBox, verifyRecordResult);
                    dataGrid.RefreshGrid();
                }
                IsShowEditRecord = false;
            }
        }
        #endregion

        #region 開窗選取紀錄使用到的方法
        public void OnOpenPicker()
        {
            ShowAontherRecordPicker = true;
        }

        public void OnPickerCompletion(ProductAdapterModel e)
        {
            if (e != null)
            {
                CurrentRecord.ExpertDirectoryId = e.Id;
                CurrentRecord.ExpertDirectoryName = e.Name;
            }
            ShowAontherRecordPicker = false;
        }
        #endregion

        #region 資料表關聯的方法
        public async Task UpdateMasterHeaderAsync(MasterRecord header)
        {
            Header = header;
            await Task.Delay(100);
            dataGrid.RefreshGrid();
        }
        #endregion

        #region 排序搜尋事件

        public void SortChanged(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, SortCondition> args)
        {
            if (dataGrid.GridIsExist() == true)
            {
                CurrentSortCondition.Id = args.Value;
                dataGrid.RefreshGrid();
            }
        }

        #endregion
        #endregion
    }
}
