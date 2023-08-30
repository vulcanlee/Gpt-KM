using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GptLibrary.Models;
using GptLibrary.Gpt;
using CommonDomain.DataModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GptLibrary.Gpts;

/// <summary>
/// 使用 Twcs GPT 服務
/// </summary>
public class TwcsGPTPromptCompletion : IGPTPromptCompletion
{
    private readonly ILogger<TwcsGPTPromptCompletion> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly OpenAIConfiguration openAIConfiguration;
    string API_KEY = "9114aa97-8ede-4f8b-8c65-d713223fe090";
    string API_SERVER = "https://ffm-trial05.twcc.ai/text-generation/api/models/generate";
    float ChatPromptCompletionTemperature = 0.3f;

    public TwcsGPTPromptCompletion(ILogger<TwcsGPTPromptCompletion> logger,
        IHttpClientFactory httpClientFactory, OpenAIConfiguration openAIConfiguration)
    {
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
        this.openAIConfiguration = openAIConfiguration;

        API_KEY = openAIConfiguration.TwcsAPI_KEY;
        API_SERVER = openAIConfiguration.TwcsGPTEndpoint;
        ChatPromptCompletionTemperature = openAIConfiguration.ChatPromptCompletionTemperature;
    }

    public async Task<string> GptAnswerQuestionAsync(string content, string prefix = "依據底下內容，回答這個問題")
    {
        string result = string.Empty;
        try
        {
            logger.LogInformation("台智雲 GPT 測試用服務啟動");
            string prompt = $"{prefix}\n\n{content}";

            HttpResponseMessage response = null;
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", API_KEY);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            GptTwccRequest embeddingRequest = new GptTwccRequest();
            embeddingRequest.inputs = prompt;
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
                    result = $"{embeddingResponse.generated_text}";
                }
                else
                {
                    result = $"API 異常狀態: " +
                        string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.RequestMessage);
                    logger.LogWarning(result);
                }
            }
            else
            {
                result = $"應用程式呼叫 API 發生異常";
                logger.LogError(result);
            }
        }
        catch (Exception ex)
        {
            result = $"發生例外異常 : {ex.Message}";
            logger.LogError(result);
        }
        return result;
    }

    public async Task<string> GptSummaryAsync(string content, string prefix = "將底下內容，產生摘要說明內容")
    {
        string result = string.Empty;
        await GptAnswerQuestionAsync(content, prefix);
        return result;
    }

}
