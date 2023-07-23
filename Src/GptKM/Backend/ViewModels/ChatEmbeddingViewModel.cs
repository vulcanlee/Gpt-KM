﻿using Backend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using CommonDomain.DataModels;
using GptLibrary.Helpers;
using GptLibrary.Models;
using Microsoft.JSInterop;
using Backend.Services.Interfaces;

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

        public ChatEmbeddingViewModel( NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor,
            OpenAIConfiguration openAIConfiguration,
            EmbeddingSearchHelper embeddingSearchHelper)
        {
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

        public async Task<List<GptEmbeddingItem>> SendQuestionAsync()
        {
            ChatEmbeddingModel.Answer = "";
            ChatEmbeddingModel.DoSearching = true;
            List<GptEmbeddingItem> gptEmbeddings = 
                await embeddingSearchHelper.SearchAsync(ChatEmbeddingModel.Question);
            ChatEmbeddingModel.DoSearching = false;
            return gptEmbeddings;
        }

        public void ShowChunkContext(SearchResult searchResult)
        {
            if (searchResult.ShowEmbeddingText == true)
            {
                searchResult.ShowEmbeddingText = false;
            }
            else
            {
                foreach (var item in ChatEmbeddingModel.SearchResult)
                {
                    item.ShowEmbeddingText = false;
                }

                searchResult.ShowEmbeddingText = true;
            }
        }

        public async Task GetAnswerAsync(SearchResult searchResult)
        {
            ChatEmbeddingModel.DoSearching = true;
            ChatEmbeddingModel.Answer = "";
            ChatEmbeddingModel.Answer = await embeddingSearchHelper
                .GetAnswerAsync(searchResult.GptEmbeddingItem.ExpertFileChunk, ChatEmbeddingModel.Question);
            ChatEmbeddingModel.DoSearching = false;
        }

        //public async Task DownloadFileAsync(SearchResult searchResult)
        //{
        //    // https://learn.microsoft.com/en-us/aspnet/core/blazor/file-downloads?view=aspnetcore-7.0
        //    string filename = searchResult.GptEmbeddingItem.ExpertFileChunk.ExpertFile.FileName;
        //    string fullfilename = searchResult.GptEmbeddingItem.ExpertFileChunk.ExpertFile.FullName;

        //    var fileStream = GetFileStream();
        //    var fileName = "log.bin";

        //    using var streamRef = new DotNetStreamReference(stream: fileStream);

        //    await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        //}

        public Stream GetFileStream(string fullfilename)
        {
            return File.OpenRead(fullfilename);
        }
    }
}
