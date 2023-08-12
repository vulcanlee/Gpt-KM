using Azure.AI.OpenAI;
using CommonDomain.DataModels;
using Domains.Models;
using GptLibrary.Gpts;
using GptLibrary.Models;
using GptLibrary.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Helpers;

public class EmbeddingSearchHelper
{
    private readonly OpenAIConfiguration openAIConfiguration;
    private readonly AdaEmbeddingVector adaEmbeddingVector;
    private readonly DavinciPromptCompletion davinciPromptCompletion;
    private readonly ILogger<EmbeddingSearchHelper> logger;
    List<GptEmbeddingItem> allDocumentsEmbedding = new();

    public EmbeddingSearchHelper(OpenAIConfiguration openAIConfiguration,
        AdaEmbeddingVector adaEmbeddingVector, DavinciPromptCompletion davinciPromptCompletion,
        ILogger<EmbeddingSearchHelper> logger)
    {
        this.openAIConfiguration = openAIConfiguration;
        this.adaEmbeddingVector = adaEmbeddingVector;
        this.davinciPromptCompletion = davinciPromptCompletion;
        this.logger = logger;
    }

    public void Reset()
    {
        allDocumentsEmbedding.Clear();
    }

    public int GetTotalCount()
    {
        return allDocumentsEmbedding.Count;
    }

    public async Task<List<GptEmbeddingCosineResultItem>> SearchAsync(string question)
    {
        List<GptEmbeddingCosineResultItem> allDocumentsCosineSimilarity = new();
        allDocumentsCosineSimilarity.Clear();
        await Task.Yield();
        float[] questionEmbedding = await adaEmbeddingVector.GetEmbeddingAsync(question);

        foreach (var item in allDocumentsEmbedding)
        {
            GptEmbeddingCosineResultItem gptEmbeddingCosineResultItem = new()
            {
                  GptEmbeddingItem = item
            };
            // calculate cosine similarity
            var v2 = item.Embedding;
            var v1 = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.DenseOfArray(questionEmbedding); ;
            var cosineSimilarity = v1.DotProduct(v2) / (v1.L2Norm() * v2.L2Norm());
            gptEmbeddingCosineResultItem.CosineSimilarity = cosineSimilarity;
            allDocumentsCosineSimilarity.Add(gptEmbeddingCosineResultItem);
        }
        allDocumentsCosineSimilarity = allDocumentsCosineSimilarity
            .OrderByDescending(x => x.CosineSimilarity).Take(10).ToList();
        return allDocumentsCosineSimilarity;
    }

    public async Task<string> GetAnswerAsync(ExpertFileChunk expertFileChunk, string question)
    {
        string result = string.Empty;
        string fileName = expertFileChunk.EmbeddingTextFileName;
        string chunkMessage = await File.ReadAllTextAsync(fileName);
        result = await davinciPromptCompletion.GptAnswerQuestionAsync(chunkMessage, $"請使用底下提示文字，回答這個問題:\"{question}\"");
        return result;
    }

    public async Task<string> GetSummaryAsync(ExpertFileChunk expertFileChunk)
    {
        string result = string.Empty;
        string fileName = expertFileChunk.EmbeddingTextFileName;
        string chunkMessage = await File.ReadAllTextAsync(fileName);
        result = await davinciPromptCompletion.GptSummaryAsync(chunkMessage);
        return result;
    }

    public async Task AddAsync(ExpertFile expertFile)
    {
        #region 建立 Embedding DB

        #region 從資料庫內，取得所以已經產生 Embedding 紀錄的檔案清單
        //ServiceResult<List<ExpertFile>> allFilesResult = await gptExpertFileService.GetAllEmbeddingAsync();
        #endregion

#if DEBUG
#endif
        try
        {
            foreach (var expertFileChunkItem in expertFile.ExpertFileChunk)
            {
                string chunkembeddingContentFileName = expertFileChunkItem.EmbeddingTextFileName;
                string chunkembeddingFileName = expertFileChunkItem.EmbeddingJsonFileName;
                int convertIndex = expertFileChunkItem.ConvertIndex;
                if (!File.Exists(chunkembeddingContentFileName)) { continue; }
                if (!File.Exists(chunkembeddingFileName)) { continue; }

                string embeddingContentContext =
                    await File.ReadAllTextAsync(chunkembeddingContentFileName);
                string embeddingContext =
                    await File.ReadAllTextAsync(chunkembeddingFileName);
                var fileEmbedding = JsonConvert.DeserializeObject<List<float>>(embeddingContext);
                float[] allValues = fileEmbedding.ToArray();
                MathNet.Numerics.LinearAlgebra.Vector<float> theEmbedding;
                theEmbedding = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.DenseOfArray(allValues);
                GptEmbeddingItem embeddingItem = new GptEmbeddingItem()
                {
                    Embedding = theEmbedding,
                    FileName = expertFileChunkItem.FullName,
                    ChunkContent = embeddingContentContext,
                    ExpertFileChunk = expertFileChunkItem
                };
                allDocumentsEmbedding.Add(embeddingItem);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"建立內嵌搜尋 {expertFile.Id} : {expertFile.FileName} 發生錯誤");
        }
        #endregion
        return;
    }

    public Task DeleteAllChunkRawFileAsync(ExpertFile expertFile)
    {
        try
        {
            foreach (var expertFileChunkItem in expertFile.ExpertFileChunk)
            {
                string chunkembeddingContentFileName = expertFileChunkItem.EmbeddingTextFileName;
                string chunkembeddingFileName = expertFileChunkItem.EmbeddingJsonFileName;
                int convertIndex = expertFileChunkItem.ConvertIndex;
                if (File.Exists(chunkembeddingContentFileName))
                {
                    try
                    {
                        File.Delete(chunkembeddingContentFileName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"刪除內嵌搜尋 {chunkembeddingContentFileName} 發生錯誤");
                    }
                }

                if (File.Exists(chunkembeddingFileName))
                {
                    try
                    {
                        File.Delete(chunkembeddingFileName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"刪除內嵌搜尋 {chunkembeddingFileName} 發生錯誤");
                    }
                }

                var itemEmbedding = allDocumentsEmbedding
                    .FirstOrDefault(x => x.ExpertFileChunk.Id == expertFileChunkItem.Id);
                if (itemEmbedding != null)
                {
                    allDocumentsEmbedding.Remove(itemEmbedding);
                }
                else
                {
                    //logger.LogWarning($"刪除內嵌搜尋集合物件 {itemEmbedding.ExpertFileChunk.Id} : {expertFile.FileName} 發生錯誤");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"刪除內嵌搜尋 {expertFile.Id} : {expertFile.FileName} 發生錯誤");
        }
        return Task.CompletedTask;
    }

    public Task DeleteExpertFileAsync(ExpertFile expertFile)
    {
        try
        {
            File.Delete(expertFile.FullName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"刪除原始資料檔案 {expertFile.FullName} 發生錯誤");
        }
        return Task.CompletedTask;
    }
}
