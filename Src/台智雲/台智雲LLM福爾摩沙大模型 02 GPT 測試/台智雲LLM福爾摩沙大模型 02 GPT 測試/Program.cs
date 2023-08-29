using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace 台智雲LLM福爾摩沙大模型_02_GPT_測試
{
    public class GptTwccRequest
    {
        public string model { get; set; } = "FFM-176B-latest";
        public string inputs { get; set; } = string.Empty;
        //public List<string> input { get; set; } = new();
    }

    public class GptTwccResponse
    {
        public List<GptTwccResponseNode> Data { get; set; } = new();
    }
    public class GptTwccResponseNode
    {
        public List<float> Embedding { get; set; } = new();
        public int index { get; set; }
    }
    public interface IGPTPromptCompletion
    {
        Task<string> GptAnswerQuestionAsync(string content, string prefix = "依據底下內容，回答這個問題");
        Task<string> GptSummaryAsync(string content, string prefix = "將底下內容，產生摘要說明內容");
    }
    public class TwcsGPTPromptCompletion: IGPTPromptCompletion
    {
        private readonly ILogger<TwcsGPTPromptCompletion> logger;
        private readonly IHttpClientFactory httpClientFactory;
        string API_KEY = "9114aa97-8ede-4f8b-8c65-d713223fe090";
        string API_SERVER = "https://ffm-trial05.twcc.ai/text-generation/api/models/generate";

        public TwcsGPTPromptCompletion(ILogger<TwcsGPTPromptCompletion> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<string> GptAnswerQuestionAsync(string content, string prefix = "依據底下內容，回答這個問題")
        {
            string result = string.Empty;
            try
            {
                logger.LogInformation("台智雲 GPT 測試用服務啟動");

                HttpResponseMessage response = null;
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("X-API-KEY", API_KEY);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                GptTwccRequest embeddingRequest = new GptTwccRequest();
                //embeddingRequest.input.Add("我想要買一台筆電");
                embeddingRequest.inputs = "我想要買一台筆電";
                var JsonContent = JsonConvert.SerializeObject(embeddingRequest);
                using (var fooContent = new StringContent(JsonContent, Encoding.UTF8, "application/json"))
                {
                    response = await client.PostAsync(API_SERVER, fooContent);
                }

                if (response != null)
                {
                    if (response.IsSuccessStatusCode == true)
                    {
                        // 取得呼叫完成 API 後的回報內容
                        String strResult = await response.Content.ReadAsStringAsync();
                        GptTwccResponse embeddingResponse = JsonConvert.DeserializeObject<GptTwccResponse>(strResult);
                        result = $"呼叫結果: {strResult}";
                        logger.LogInformation(result);
                    }
                    else
                    {
                        result = $"API 異常狀態: " +
                            string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.RequestMessage);
                        logger.LogInformation(result);
                    }
                }
                else
                {
                    result = $"應用程式呼叫 API 發生異常";
                    logger.LogInformation(result);
                }
            }
            catch (Exception ex)
            {
                result = $"發生例外異常 : {ex.Message}";
                logger.LogInformation(result);
            }
            return result;
        }

        public async Task<string> GptSummaryAsync(string content, string prefix = "將底下內容，產生摘要說明內容")
        {
            string result = string.Empty;
            await GptAnswerQuestionAsync(content, prefix);
            return result;
        }

        public async Task Run()
        {
            try
            {
                logger.LogInformation("台智雲 GPT 測試用服務啟動");

                HttpResponseMessage response = null;
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("X-API-KEY", API_KEY);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                GptTwccRequest embeddingRequest = new GptTwccRequest();
                //embeddingRequest.input.Add("我想要買一台筆電");
                embeddingRequest.inputs = "我想要買一台筆電";
                var JsonContent = JsonConvert.SerializeObject(embeddingRequest);
                using (var fooContent = new StringContent(JsonContent, Encoding.UTF8, "application/json"))
                {
                    response = await client.PostAsync(API_SERVER, fooContent);
                }

                if (response != null)
                {
                    if (response.IsSuccessStatusCode == true)
                    {
                        // 取得呼叫完成 API 後的回報內容
                        String strResult = await response.Content.ReadAsStringAsync();
                        GptTwccResponse embeddingResponse = JsonConvert.DeserializeObject<GptTwccResponse>(strResult);
                        logger.LogInformation($"呼叫結果: {strResult}");
                    }
                    else
                    {
                        logger.LogInformation($"API 異常狀態: " +
                            string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.RequestMessage));
                    }
                }
                else
                {
                    logger.LogInformation($"應用程式呼叫 API 發生異常");
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation($"發生例外異常 : {ex.Message}");
            }
        }

    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                           .ConfigureServices((hostContext, services) =>
                           {
                               services.AddHttpClient();
                               services.AddTransient<IGPTPromptCompletion, TwcsGPTPromptCompletion>();
                           }).UseConsoleLifetime()
                           .ConfigureLogging(logging =>
                           {
                               // 指定日誌寫入目的地
                               logging.AddConsole();
                           });

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var myService = services.GetRequiredService<TwcsGPTPromptCompletion>();
                    await myService.Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Occured");
                }
            }
        }
    }
}