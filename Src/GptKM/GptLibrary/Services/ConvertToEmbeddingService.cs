using EntityModel.Entities;
using GptLibrary.Converts;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Models;
using System.Dynamic;

namespace GptLibrary.Services;

/// <summary>
/// 將 Chunk 文字內容轉換成為 Embedding 向量
/// </summary>
public class ConvertToEmbeddingService
{
    private readonly AdaEmbeddingVector adaEmbeddingVector;
    private readonly GptExpertFileService gptExpertFileService;
    private readonly ConvertFileModelService convertFileModelService;

    public ConvertToEmbeddingService(AdaEmbeddingVector adaEmbeddingVector,
        GptExpertFileService gptExpertFileService,
        ConvertFileModelService convertFileModelService)
    {
        this.adaEmbeddingVector = adaEmbeddingVector;
        this.gptExpertFileService = gptExpertFileService;
        this.convertFileModelService = convertFileModelService;
    }

    /// <summary>
    /// 將指定的檔案 Chunk，把文字內容轉換成為 Embedding
    /// </summary>
    /// <param name="expertFile"></param>
    public async Task ConvertAsync(ExpertFile expertFile, ConvertFileModel convertFile, int index)
    {
        var expertFileResult = await gptExpertFileService.GetAsync(expertFile.FullName);
        expertFile = expertFileResult.Payload;

        ConvertFileSplitItemModel convertFileItemModel = convertFile.ConvertFileSplitItems.FirstOrDefault(x => x.Index == index)!;
        string chunkembeddingFileName = convertFileItemModel.EmbeddingJsonFileName;
        string content = convertFileItemModel.SourceText;
        //float[] embeddings = await adaEmbeddingVector.GetEmbeddingAsync(content);
        //convertFileItemModel.Embedding = embeddings.ToList();

        await convertFileModelService
            .ExportEmbeddingJsonAsync(expertFile, convertFile, index);
        await convertFileModelService
            .ExportEmbeddingTextAsync(expertFile, convertFile, index);

        expertFile.ProcessingStatus = CommonDomain.Enums.ExpertFileStatusEnum.ToEmbedding;
        await gptExpertFileService.UpdateAsync(expertFile);
    }
}
