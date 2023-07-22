using Backend.AdapterModels;
using Backend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using BAL.Helpers;
using CommonDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Backend.Helpers;
using Backend.Services.Interfaces;
using Syncfusion.Blazor.Inputs;
using CommonDomain.DataModels;

namespace Backend.ViewModels
{
    public class RootFileUploadViewModel
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
        public RootFileUploadModel RootFileUploadModel { get; set; } = new RootFileUploadModel();
        public EditContext LocalEditContext { get; set; }
        public IRootFileUploadService RootFileUploadService { get; }
        public NavigationManager NavigationManager { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public bool Relogin { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string ProcessingLog { get; set; } = string.Empty;
        public string UploadFileName { get; set; } = string.Empty;
        public bool PreCheckHasError { get; set; } = false;
        public bool IsLoad { get; set; } = false;
        public Action<string> ShowStatusHandler;
        private readonly OpenAIConfiguration openAIConfiguration;

        public RootFileUploadViewModel(IRootFileUploadService rootFileUploadService,
            NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor,
            OpenAIConfiguration openAIConfiguration)
        {
            RootFileUploadService = rootFileUploadService;
            NavigationManager = navigationManager;
            HttpContextAccessor = httpContextAccessor;
            this.openAIConfiguration = openAIConfiguration;
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

        public async Task GetUploadFileAsync(List<UploadFiles> uploadFiles)
        {
            UploadFiles uploadFile = uploadFiles.FirstOrDefault();
            MemoryStream inputFileStream = uploadFile.Stream;
            Syncfusion.Blazor.Inputs.FileInfo fileInfo = uploadFile.FileInfo;
            if(fileInfo.StatusCode == "2")
            {
                #region 上傳成功
                #region 建立檔案名稱
                var expertDirectory = await RootFileUploadService
                    .GetDefaultExpertDirectoryAsync(openAIConfiguration.DefaultExpertDirectoryName);
                string fileName = Path.Combine(expertDirectory.SourcePath, fileInfo.Name);
                var expertFile = await RootFileUploadService.GetExpertFileAsync(fileName, expertDirectory);
                #endregion
                using (FileStream file = new FileStream(fileName, FileMode.Create, System.IO.FileAccess.Write))
                {
                    await uploadFile.File.OpenReadStream(long.MaxValue).CopyToAsync(file);
                    file.Close();
                }
                #endregion
            }
        }

    }
}
