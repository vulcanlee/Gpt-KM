using Backend.ViewModels;
using Backend.Services;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using Backend.Events;
using Backend.Models;
using Backend.Services.Interfaces;
using CommonDomain.DataModels;
using GptLibrary.Converts;
using GptLibrary.Gpts;
using GptLibrary.Services;
using GptLibrary.Helpers;

namespace Backend.Helpers
{
    public static class CustomDependency
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            #region 註冊服務
            services.AddTransient<IChatEmbeddingService, ChatEmbeddingService>();
            services.AddTransient<IRootFileUploadService, RootFileUploadService>();
            services.AddTransient<IExpertDirectoryService, ExpertDirectoryService>();
            services.AddTransient<IExpertFileService, ExpertFileService>();
            services.AddTransient<IExpertFileChunkService, ExpertFileChunkService>();

            services.AddTransient<IExportDataService, ExportDataService>();
            services.AddTransient<IExceptionRecordService, ExceptionRecordService>();
            services.AddTransient<IMailQueueService, MailQueueService>();
            services.AddTransient<IMyUserPasswordHistoryService, MyUserPasswordHistoryService>();
            services.AddTransient<IPasswordPolicyService, PasswordPolicyService>();
            services.AddTransient<IAccountPolicyService, AccountPolicyService>();

            services.AddTransient<ISystemLogService, SystemLogService>();
            services.AddTransient<IChangePasswordService, ChangePasswordService>();
            services.AddTransient<IMenuRoleService, MenuRoleService>();
            services.AddTransient<IMenuDataService, MenuDataService>();
            services.AddTransient<IMyUserService, MyUserService>();
            services.AddTransient<DatabaseInitService>();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderMasterService, OrderMasterService>();
            services.AddTransient<IOrderItemService, OrderItemService>();

            #endregion

            #region 註冊 ViewModel
            services.AddTransient<ChatDocumentViewModel>();
            services.AddTransient<FileProcessingStatusViewModel>();
            services.AddTransient<ChatEmbeddingViewModel>();
            services.AddTransient<RootFileUploadViewModel>();
            services.AddTransient<ExpertFileViewModel>();
            services.AddTransient<ExpertDirectoryViewModel>();

            services.AddTransient<ExceptionRecordViewModel>();
            services.AddTransient<MailQueueViewModel>();
            services.AddTransient<AccountPolicyViewModel>();

            services.AddTransient<SystemLogViewModel>();
            services.AddTransient<ChangePasswordViewModel>();
            services.AddTransient<MenuRoleViewModel>();
            services.AddTransient<MenuDataViewModel>();
            services.AddTransient<MyUserViewModel>();

            services.AddTransient<OrderMasterViewModel>();
            services.AddTransient<ProductViewModel>();
            services.AddTransient<OrderItemViewModel>();
            #endregion

            #region GPT Service
            services.AddTransient<GPT35PromptCompletion>();
            services.AddTransient<SearchCollectionBuilderHelper>();
            services.AddSingleton<EmbeddingSearchHelper>();
            services.AddSingleton<OpenAIConfiguration>();
            services.AddTransient<ConverterToTextFactory>();
            services.AddTransient<ConvertFileExtensionMatchService>();
            services.AddTransient<SyncDirectoryService>();
            services.AddTransient<SyncFilesToDatabaseService>();
            services.AddTransient<ConvertToTextService>();
            services.AddTransient<BuildFilenameService>();
            services.AddTransient<ConvertFileModelService>();
            services.AddTransient<ConvertToEmbeddingService>();
            services.AddTransient<AdaEmbeddingVector>();
            services.AddTransient<DavinciPromptCompletion>();
            services.AddTransient<GptExpertDirectoryService>();
            services.AddTransient<GptExpertFileService>();
            services.AddTransient<GptExpertFileChunkService>();
            services.AddTransient<SyncProcessingService>();
            #endregion

            #region 其他服務註冊
            services.AddTransient<ChatDocumentService>();
            services.AddTransient<FileProcessingStatusService>();
            services.AddScoped<CurrentUser>();
            services.AddScoped<UserHelper>();
            services.AddSingleton<SystemBroadcast>();
            services.AddTransient<ImportDataHelper>();
            services.AddTransient<TranscationResultHelper>();
            services.AddTransient<SystemLogHelper>();
            services.AddScoped<BlazorAppContext>();
            services.AddScoped<IEventAggregator, EventAggregator>();
            #region 產品授權服務註冊
            services.AddSingleton<ProductLicense>();
            #endregion

            #endregion

            return services;
        }
    }
}
