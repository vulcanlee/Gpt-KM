﻿using Backend.AdapterModels;
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
using Backend.Services;
using GptLibrary.Helpers;
using GptLibrary.Models;

namespace Backend.ViewModels
{
    public class ChatEmbeddingViewModel
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
        public ChatEmbeddingModel ChatEmbeddingModel { get; set; } = new ChatEmbeddingModel();
        public EditContext LocalEditContext { get; set; }
        public IChatEmbeddingService ChatEmbeddingService { get; }
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
        private readonly EmbeddingSearchHelper embeddingSearchHelper;

        public ChatEmbeddingViewModel(IChatEmbeddingService ChatEmbeddingService,
            NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor,
            OpenAIConfiguration openAIConfiguration,
            EmbeddingSearchHelper embeddingSearchHelper)
        {
            ChatEmbeddingService = ChatEmbeddingService;
            NavigationManager = navigationManager;
            HttpContextAccessor = httpContextAccessor;
            this.openAIConfiguration = openAIConfiguration;
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

        public async Task SendQuestionAsync()
        {
            List<GptEmbeddingItem> gptEmbeddings = 
                await embeddingSearchHelper.SearchAsync(ChatEmbeddingModel.Question);
        }

    }
}
