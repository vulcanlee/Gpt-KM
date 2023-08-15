﻿using Backend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using CommonDomain.DataModels;
using GptLibrary.Helpers;
using GptLibrary.Models;
using Microsoft.JSInterop;
using Backend.Services.Interfaces;
using Backend.Services;
using Backend.Interfaces;

namespace Backend.ViewModels
{
    public class ChatDocumentViewModel
    {

        public ChatDocumentSpecificItem ChatDocumentSpecificItem { get; set; } = new ChatDocumentSpecificItem();
        public ChatDocumentModel ChatDocumentModel { get; set; } = new ChatDocumentModel();
        /// <summary>
        /// 這個元件整體的通用介面方法
        /// </summary>
        IRazorPage thisView;

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
        public bool ShowUploadDialog { get; set; } = false;
        public bool ShowFileProcessingStatusDialog { get; set; } = false;
        public bool ShowChatDocumentDialog { get; set; } = false;
        public bool IsLoad { get; set; } = false;
        public Action<string> ShowStatusHandler;
        private readonly EmbeddingSearchHelper embeddingSearchHelper;
        private readonly ChatDocumentService chatDocumentService;

        public ChatDocumentViewModel(NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor,
            EmbeddingSearchHelper embeddingSearchHelper, ChatDocumentService chatDocumentService)
        {
            NavigationManager = navigationManager;
            HttpContextAccessor = httpContextAccessor;
            this.embeddingSearchHelper = embeddingSearchHelper;
            this.chatDocumentService = chatDocumentService;
        }

        #region 初始化
        /// <summary>
        /// 將會於 生命週期事件 OnInitialized / OnAfterRenderAsync 觸發此方法
        /// </summary>
        /// <param name="razorPage">當前元件的物件</param>
        /// <param name="dataGrid">當前 Grid 的元件</param>
        public void Setup(IRazorPage razorPage)
        {
            thisView = razorPage;
        }
        #endregion


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

        public async Task<List<GptEmbeddingCosineResultItem>> SendQuestionAsync()
        {
            ChatEmbeddingModel.DoSearching = true;
            ChatDocumentModel.AddUserContent(ChatEmbeddingModel.Question);

            List<GptEmbeddingCosineResultItem> gptEmbeddings =
                await embeddingSearchHelper
                .SearchChatDocumentAsync(ChatEmbeddingModel.Question, ChatDocumentSpecificItem.ExpertFile);
            GptEmbeddingCosineResultItem gptEmbeddingCosineResultItem = gptEmbeddings.FirstOrDefault();
            if (gptEmbeddingCosineResultItem != null)
            {
                #region 將內嵌的原文與問題，送給 GPT 回答
                ChatEmbeddingModel.Answer = await embeddingSearchHelper
                    .GetAnswerAsync(gptEmbeddingCosineResultItem.GptEmbeddingItem.ExpertFileChunk,
                    ChatEmbeddingModel.Question);
                ChatDocumentModel
                    .AddGPTContent(ChatEmbeddingModel.Answer);
                #endregion
            }
            else
            {
                ChatDocumentModel
                    .AddGPTContent($"在系統資料庫內，找不到相關內嵌檔案 {ChatDocumentSpecificItem.ExpertFile.FullName}");
            }
            ChatEmbeddingModel.DoSearching = false;
            ChatEmbeddingModel.Question = "";
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
            ChatEmbeddingModel.Answer = "";
            foreach (var item in ChatEmbeddingModel.SearchResult)
            {
                item.Answer = "";
                item.DoAnswerSearching = false;
            }

            searchResult.DoAnswerSearching = true;
            ChatEmbeddingModel.Answer = await embeddingSearchHelper
                .GetAnswerAsync(searchResult.GptEmbeddingItem.ExpertFileChunk, ChatEmbeddingModel.Question);
            searchResult.Answer = ChatEmbeddingModel.Answer;
            searchResult.DoAnswerSearching = false;
        }

        public async Task GetSummaryAsync(SearchResult searchResult)
        {
            ChatEmbeddingModel.Answer = "";
            foreach (var item in ChatEmbeddingModel.SearchResult)
            {
                item.Answer = "";
                item.DoAnswerSearching = false;
            }

            searchResult.DoAnswerSearching = true;
            ChatEmbeddingModel.Answer = await embeddingSearchHelper
                .GetSummaryAsync(searchResult.GptEmbeddingItem.ExpertFileChunk);
            searchResult.Answer = ChatEmbeddingModel.Answer;
            searchResult.DoAnswerSearching = false;
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

        public async Task OpenUploadFileAsync()
        {
            ShowUploadDialog = true;
            await Task.Yield();
        }

        public async Task CloseUploadFileAsync()
        {
            ShowUploadDialog = false;
            await Task.Yield();
        }

        public async Task OpenFileProcessingStatusAsync()
        {
            ShowFileProcessingStatusDialog = true;
            await Task.Yield();
        }

        public async Task CloseFileProcessingStatusAsync()
        {
            ShowFileProcessingStatusDialog = false;
            await Task.Yield();
        }

        public async Task OpenShowChatDocumentAsync(SearchResult searchResult)
        {
            ShowChatDocumentDialog = true;
            await Task.Yield();
        }

        public async Task CloseShowChatDocumentAsync()
        {
            ShowChatDocumentDialog = false;
            await Task.Yield();
        }
    }
}