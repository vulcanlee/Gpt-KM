using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BAL.Helpers;
using Microsoft.Extensions.Options;
using Backend.Models;
using CommonDomain.DataModels;
using Backend.Events;
using Backend.Helpers;
using Backend.AdapterModels;
using GptLibrary.Helpers;
using Domains.Models;
using GptLibrary.Services;

namespace Backend.Services
{
    public class SyncSearchIndexService : IHostedService
    {
        public SyncSearchIndexService(ILogger<SendingMailHostedService> logger,
            IServer server, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory,
            SystemBroadcast systemBroadcast,
            BackgroundExecuteMode backgroundExecuteMode,
            EmbeddingSearchHelper embeddingSearchHelper)
        {
            Logger = logger;
            Server = server;
            Configuration = configuration;
            ServiceScopeFactory = serviceScopeFactory;
            SystemBroadcast = systemBroadcast;
            BackgroundExecuteMode = backgroundExecuteMode;
            this.embeddingSearchHelper = embeddingSearchHelper;
        }

        public ILogger<SendingMailHostedService> Logger { get; }
        public IServer Server { get; }
        public IConfiguration Configuration { get; }
        public IServiceScopeFactory ServiceScopeFactory { get; }
        public SystemBroadcast SystemBroadcast { get; }
        public BackgroundExecuteMode BackgroundExecuteMode { get; }

        DateTime StartupTime = DateTime.Now;
        Task PasswordPolicyTask;
        CancellationTokenSource cancellationTokenSource = new();
        private readonly EmbeddingSearchHelper embeddingSearchHelper;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int smtpExceptionTimes = 0;
            cancellationTokenSource = new CancellationTokenSource();
            Logger.LogInformation($"同步內嵌搜尋索引 服務開始啟動");
            int SyncSearchIndexInterval = Convert.ToInt32(Configuration[AppSettingHelper.SyncSearchIndexInterval]);

            var backgroundService = Task.Run(async () =>
            {
                #region 進行 同步內嵌搜尋索引
                try
                {
                    //await Task.Delay(120000, cancellationTokenSource.Token);

                    StartupTime = DateTime.Now;
                    Random random = new Random();
                    var firstDelay = random.Next(60 * 1000, 2 * 60 * 1000);

#if DEBUG
                    await Task.Delay(5000);
#else
                    await Task.Delay(firstDelay);
#endif

                    var scope = ServiceScopeFactory.CreateScope();
                    SearchCollectionBuilderHelper searchCollectionBuilderHelper =
                    scope.ServiceProvider.GetRequiredService<SearchCollectionBuilderHelper>();
                    GptExpertDirectoryService gptExpertDirectoryService=
                    scope.ServiceProvider.GetRequiredService<GptExpertDirectoryService>();
                    SyncProcessingService syncProcessingService =
                    scope.ServiceProvider.GetRequiredService<SyncProcessingService>();
                    await searchCollectionBuilderHelper.BuildAsync();

                    while (cancellationTokenSource.Token.IsCancellationRequested == false)
                    {
                        #region 若在進行資料庫重建與初始化的時候，需要暫緩執行背景工作
                        while (BackgroundExecuteMode.IsInitialization == true)
                        {
                            await Task.Delay(60000, cancellationTokenSource.Token);
                        }
                        #endregion

                        var dateOffset = DateTime.UtcNow.AddHours(8);
                        TimeSpan timeSpan = DateTime.Now - StartupTime;

                        try
                        {
                            Logger.LogDebug($"同步內嵌搜尋索引開始啟動掃描與同步工作");

                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            #region 處理 內嵌搜尋索引 同步工作
                            ExpertDirectory expertDirectory = null;

                            #region 取得專家目錄
                            var expertDirectoryResult = await gptExpertDirectoryService.GetAsync("本機測試用");
                            if (expertDirectoryResult.Status)
                            {
                                expertDirectory = expertDirectoryResult.Payload;
                            }
                            else
                            {
                                expertDirectory = new ExpertDirectory()
                                {
                                    Name = "本機測試用",
                                    SourcePath = @"C:\Home\Source",
                                    ConvertPath = @"C:\Home\Convert",
                                };
                                var createResult = await gptExpertDirectoryService.CreateAsync(expertDirectory);
                                if (createResult.Status)
                                {
                                    expertDirectory = createResult.Payload;
                                }
                                else
                                {
                                    throw new Exception(createResult.Message);
                                }
                            }
                            #endregion

                            #region 檢查專家目錄
                            await syncProcessingService.BeginSyncDirectoryAsync(expertDirectory);
                            #endregion
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            smtpExceptionTimes++;
                            Logger.LogWarning(ex, $"同步內嵌搜尋索引 發生例外異常 ({smtpExceptionTimes})");
                        }

                        await Task.Delay(SyncSearchIndexInterval, cancellationTokenSource.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Logger.LogInformation($"同步內嵌搜尋索引 服務準備正常離開中");
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, $"同步內嵌搜尋索引 服務產生例外異常");
                }
                #endregion
            });
            PasswordPolicyTask = backgroundService;

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();
            for (int i = 0; i < 10; i++)
            {
                if (PasswordPolicyTask.IsCompleted == true)
                    break;
#pragma warning disable CA2016 // 將 'CancellationToken' 參數傳遞給使用該參數的方法
                await Task.Delay(500);
#pragma warning restore CA2016 // 將 'CancellationToken' 參數傳遞給使用該參數的方法
            }
            TimeSpan timeSpan = DateTime.Now - StartupTime;
            await Console.Out.WriteLineAsync($"同步內嵌搜尋索引 服務即將停止，共花費 {timeSpan}");

            return;
        }
    }
}
