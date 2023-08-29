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

namespace GptLibrary.Gpts
{
    /// <summary>
    /// 使用 OpenAI Ada Embedding 服務
    /// </summary>
    public class AdaEmbeddingVector : IAdaEmbeddingVector
    {
        private readonly OpenAIConfiguration openAIConfiguration;

        public AdaEmbeddingVector(OpenAIConfiguration openAIConfiguration)
        {
            this.openAIConfiguration = openAIConfiguration;
        }
        /// <summary>
        /// 取得指定的文字內容的 Embedding
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public async Task<float[]> GetEmbeddingAsync(string doc)
        {
            List<float> embeddings = new List<float>();
            #region 使用 Azure.AI.OpenAI 套件來 OpenAIClient 物件
            // Todo : 這裡要修改使用 appsetting 方式來取得
            var apiKey = openAIConfiguration.AzureOpenAIKey;
            string endpoint = openAIConfiguration.AzureOpenAIEndpoint;
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            #endregion

            string deploymentName = openAIConfiguration.TextEmbeddingAdaModelName;

            EmbeddingsOptions embeddingsOptions = new EmbeddingsOptions(doc);
            try
            {
                Response<Embeddings> response = await client.GetEmbeddingsAsync(deploymentName, embeddingsOptions);

                if (response != null)
                {
                    var itemData = response.Value.Data.FirstOrDefault();
                    embeddings = itemData.Embedding.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return embeddings.ToArray();
        }
    }
}
