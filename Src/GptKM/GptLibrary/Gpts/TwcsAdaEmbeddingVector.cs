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

namespace GptLibrary.Gpts
{
    /// <summary>
    /// 使用 Twcs Ada Embedding 服務
    /// </summary>
    public class TwcsAdaEmbeddingVector : IAdaEmbeddingVector
    {
        private readonly ILogger<TwcsAdaEmbeddingVector> logger;
        private readonly IHttpClientFactory httpClientFactory;
        string API_KEY = "9114aa97-8ede-4f8b-8c65-d713223fe090";
        string API_SERVER = "https://ffm-trial05.twcc.ai/embeddings/api/embeddings";

        public TwcsAdaEmbeddingVector(ILogger<TwcsAdaEmbeddingVector> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<float[]> GetEmbeddingAsync(string doc)
        {
            List<float> result = new();
            try
            {
                logger.LogInformation("台智雲 Embedding 測試用服務啟動");

                HttpResponseMessage response = null;
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("X-API-KEY", API_KEY);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                EmbeddingTwccRequest embeddingRequest = new EmbeddingTwccRequest();
                embeddingRequest.input.Add("我想要買一台筆電");
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
                        EmbeddingTwccResponse embeddingResponse = JsonConvert.DeserializeObject<EmbeddingTwccResponse>(strResult);
                        result = embeddingResponse.Data[0].Embedding;
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
            return result.ToArray();
        }

        public async Task Run()
        {
            try
            {
                logger.LogInformation("台智雲 Embedding 測試用服務啟動");

                HttpResponseMessage response = null;
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("X-API-KEY", API_KEY);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                EmbeddingTwccRequest embeddingRequest = new EmbeddingTwccRequest();
                embeddingRequest.input.Add("我想要買一台筆電");
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
                        EmbeddingTwccResponse embeddingResponse = JsonConvert.DeserializeObject<EmbeddingTwccResponse>(strResult);
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
}
