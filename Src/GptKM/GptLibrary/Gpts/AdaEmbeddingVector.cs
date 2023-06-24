using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GptLibrary.Models;
using GptLibrary.Gpt;

namespace GptLibrary.Gpts
{
    /// <summary>
    /// 使用 OpenAI Ada Embedding 服務
    /// </summary>
    public class AdaEmbeddingVector
    {
        public AdaEmbeddingVector()
        {
            
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
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string endpoint = "https://openailabtw.openai.azure.com/";
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            #endregion

            string deploymentName = "text-embedding-ada-002";

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
