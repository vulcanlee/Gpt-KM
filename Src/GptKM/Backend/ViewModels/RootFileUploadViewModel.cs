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

        public RootFileUploadViewModel(IRootFileUploadService RootFileUploadService,
            NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor)
        {
            RootFileUploadService = RootFileUploadService;
            NavigationManager = navigationManager;
            HttpContextAccessor = httpContextAccessor;
        }
        public void OnEditContestChanged(EditContext context)
        {
            LocalEditContext = context;
        }
        public string PasswordStrengthName { get; set; }
        public async Task OnSaveAsync()
        {
            Relogin = false;
            var PasswordStrength = PasswordCheck.GetPasswordStrength(RootFileUploadModel.NewPassword);
            PasswordStrengthName = PasswordStrength.ToString();

            MyUserAdapterModel myUserAdapterModel = new MyUserAdapterModel();
            #region 進行 Form Validation 檢查驗證作業
            if (LocalEditContext.Validate() == false)
            {
                return;
            }
            #endregion

            #region 其他資料完整性驗證
            if (RootFileUploadModel.NewPasswordAgain != RootFileUploadModel.NewPassword)
            {
                MessageBox.Show("400px", "200px",
                    ErrorMessageMappingHelper.Instance.GetErrorMessage(ErrorMessageEnum.警告),
                    ErrorMessageMappingHelper.Instance.GetErrorMessage(ErrorMessageEnum.新密碼2次輸入須相同),
                    CloseMessageBox);
                return;
            }
            else
            {
                myUserAdapterModel = await RootFileUploadService.GetCurrentUser();
                if (myUserAdapterModel == null)
                {
                    MessageBox.Show("400px", "200px",
                        ErrorMessageMappingHelper.Instance.GetErrorMessage(ErrorMessageEnum.警告),
                        ErrorMessageMappingHelper.Instance.GetErrorMessage(ErrorMessageEnum.使用者不存在),
                        CloseMessageBox);
                    return;
                }
            }
            #endregion

            string msg = await RootFileUploadService
                .CheckWetherCanRootFileUpload(myUserAdapterModel, RootFileUploadModel.NewPassword);
            if (string.IsNullOrEmpty(msg) == false)
            {
                MessageBox.Show("400px", "200px",
                    ErrorMessageMappingHelper.Instance.GetErrorMessage(ErrorMessageEnum.警告), msg,
                        CloseMessageBox);
                return;
            }

            #region 進行密碼變更
            await RootFileUploadService.RootFileUpload(myUserAdapterModel, RootFileUploadModel.NewPassword,
                HttpContextAccessor.GetConnectionIP());
            Relogin = true;
            MessageBox.Show("400px", "200px",
                ErrorMessageMappingHelper.Instance.GetErrorMessage(ErrorMessageEnum.警告),
                ErrorMessageMappingHelper.Instance.GetErrorMessage(ErrorMessageEnum.密碼已經變更成功),
                        CloseMessageBox);

            #endregion
        }

        public async Task CloseMessageBox()
        {
            MessageBox.Hidden();
            await Task.Yield();
            if (Relogin)
                NavigationManager.NavigateTo("/Logout", true);
        }
    }
}
