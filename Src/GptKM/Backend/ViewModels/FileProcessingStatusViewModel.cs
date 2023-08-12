using Backend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using CommonDomain.DataModels;
using GptLibrary.Helpers;
using GptLibrary.Models;
using Microsoft.JSInterop;
using Backend.Services.Interfaces;
using Backend.Services;

namespace Backend.ViewModels
{
    public class FileProcessingStatusViewModel
    {
        #region 訊息說明之對話窗使用的變數
        /// <summary>
        /// 確認對話窗設定
        /// </summary>
        public ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
        /// <summary>
        /// 訊息對話窗設定
        /// </summary>
        public MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
        #endregion
        public EditContext LocalEditContext { get; set; }
        public NavigationManager NavigationManager { get; }
        public bool Relogin { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string ProcessingLog { get; set; } = string.Empty;
        public string UploadFileName { get; set; } = string.Empty;
        public bool PreCheckHasError { get; set; } = false;
        public bool ShowUploadDialog { get; set; } = false;
        public bool IsLoad { get; set; } = false;
        public Action<string> ShowStatusHandler;
        private readonly FileProcessingStatusService fileProcessingStatusService;
        private readonly EmbeddingSearchHelper embeddingSearchHelper;

        public FileProcessingStatusViewModel(NavigationManager navigationManager,
            FileProcessingStatusService fileProcessingStatusService,
            EmbeddingSearchHelper embeddingSearchHelper)
        {
            NavigationManager = navigationManager;
            this.fileProcessingStatusService = fileProcessingStatusService;
            this.embeddingSearchHelper = embeddingSearchHelper;
        }
        public void OnEditContestChanged(EditContext context)
        {
            LocalEditContext = context;
        }
        public string PasswordStrengthName { get; set; }

        public async Task CloseMessageBox()
        {
            MessageBox.Hidden();
            await Task.Yield();
            if (Relogin)
                NavigationManager.NavigateTo("/Logout", true);
        }

        public async Task<FileProcessingInformation> GetFileProcessingInformation()
        {
            var result = await fileProcessingStatusService.Get();
            result.已經讀入內嵌數量 = embeddingSearchHelper.GetTotalCount();
            return result;
        }
    }
}
