using CommonDomain.DataModels;
using Domains.Models;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Helpers;
using GptLibrary.Models;
using System.Dynamic;

namespace GptLibrary.Services;

/// <summary>
/// 將 Chunk 文字內容轉換成為 Embedding 向量
/// </summary>
public class ConvertToEmbeddingService
{
    private readonly AIEngineFactory aiEngineFactory;
    private readonly OpenAIConfiguration openAIConfiguration;
    private readonly IAdaEmbeddingVector adaEmbeddingVector;
    private readonly GptExpertFileService gptExpertFileService;
    private readonly GptExpertFileChunkService gptExpertFileChunkService;
    private readonly ConvertFileModelService convertFileModelService;
    private readonly EmbeddingSearchHelper embeddingSearchHelper;

    public ConvertToEmbeddingService(AIEngineFactory aiEngineFactory,
        OpenAIConfiguration openAIConfiguration,
        GptExpertFileService gptExpertFileService,
        GptExpertFileChunkService gptExpertFileChunkService,
        ConvertFileModelService convertFileModelService,
        EmbeddingSearchHelper embeddingSearchHelper)
    {
        this.aiEngineFactory = aiEngineFactory;
        this.openAIConfiguration = openAIConfiguration;
        this.adaEmbeddingVector = aiEngineFactory.CreateEmbeddingModel(openAIConfiguration.AIEngine);
        this.gptExpertFileService = gptExpertFileService;
        this.gptExpertFileChunkService = gptExpertFileChunkService;
        this.convertFileModelService = convertFileModelService;
        this.embeddingSearchHelper = embeddingSearchHelper;
    }

    /// <summary>
    /// 將指定的檔案 Chunk，把文字內容轉換成為 Embedding
    /// </summary>
    /// <param name="expertFile"></param>
    public async Task ConvertAsync(ExpertFile expertFile, ConvertFileModel convertFile, int index)
    {
        var expertFileResult = await gptExpertFileService.GetAsync(expertFile.FullName);
        expertFile = expertFileResult.Payload;

        ConvertFileSplitItemModel convertFileItemModel = convertFile
            .ConvertFileSplitItems.FirstOrDefault(x => x.Index == index)!;
        string chunkembeddingFileName = convertFileItemModel.EmbeddingJsonFileName;
        string content = convertFileItemModel.SourceText;

        #region 產生 Embedding 向量
        float[] embeddings = await adaEmbeddingVector.GetEmbeddingAsync(content);
        convertFileItemModel.Embedding = embeddings.ToList();
        #endregion

        await convertFileModelService
            .ExportEmbeddingJsonAsync(expertFile, convertFile, index);
        await convertFileModelService
            .ExportEmbeddingTextAsync(expertFile, convertFile, index);

        expertFile.ProcessingStatus = CommonDomain.Enums.ExpertFileStatusEnum.ToEmbedding;
        await gptExpertFileService.UpdateAsync(expertFile);

        #region 產生 Chunk 紀錄
        Tokenizer tokenizer = new Tokenizer();

        var expertFileChunkResult = await gptExpertFileChunkService.GetAsync(expertFile, index);
        var tokenSize = tokenizer.CountToken(content);
        var embeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(tokenSize);

        if (expertFileChunkResult.Status == true)
        {
            #region 修改 Chunk 紀錄
            var expertFileChunk = expertFileChunkResult.Payload;
            expertFileChunk.Size = content.Length;
            expertFileChunk.TokenSize = tokenSize;
            expertFileChunk.EmbeddingCost = embeddingCost;
            expertFileChunk.EmbeddingJsonFileName = convertFileItemModel.EmbeddingJsonFileName;
            expertFileChunk.EmbeddingTextFileName = convertFileItemModel.EmbeddingTextFileName;
            await gptExpertFileChunkService.UpdateAsync(expertFileChunk);
            #endregion
        }
        else
        {
            #region 新增 Chunk 紀錄
            var expertFileChunk = new ExpertFileChunk()
            {
                 DirectoryName = expertFile.DirectoryName,
                 ExpertFileId = expertFile.Id,
                 ConvertIndex = index,
                 TokenSize = tokenSize,
                 EmbeddingCost = embeddingCost,
                 FullName = expertFile.FullName,
                 FileName = expertFile.FileName,
                 Size = content.Length,
                 EmbeddingJsonFileName = convertFileItemModel.EmbeddingJsonFileName,
                 EmbeddingTextFileName = convertFileItemModel.EmbeddingTextFileName
            };
            await gptExpertFileChunkService.CreateAsync(expertFileChunk);
            #endregion
        }
        #endregion
    }
}
