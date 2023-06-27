﻿using EntityModel.Entities;
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

    public ConvertToEmbeddingService(AdaEmbeddingVector adaEmbeddingVector,
        GptExpertFileService gptExpertFileService)
    {
        this.adaEmbeddingVector = adaEmbeddingVector;
        this.gptExpertFileService = gptExpertFileService;
    }

    /// <summary>
    /// 將指定的檔案 Chunk，把文字內容轉換成為 Embedding
    /// </summary>
    /// <param name="expertFile"></param>
    public async Task ConvertAsync(ExpertFile expertFile, ConvertFileModel convertFile, int index)
    {
        string chunkembeddingFileName = Path
                        .Combine(expertFile.FullName, $"{index}{GptConstant.ConvertToEmbeddingTextFileExtension}");
        //string content =await File.ReadAllTextAsync(chunkembeddingFileName);
        string content = convertFile.ConvertFileSplitItems[index-1].SourceText;
        float[] embeddings = await adaEmbeddingVector.GetEmbeddingAsync(content);
        ConvertFileSplitItemModel convertFileItemModel = convertFile.ConvertFileSplitItems.FirstOrDefault(x => x.Index == index)!;
        convertFileItemModel.Embedding = embeddings.ToList();

        var expertFileResult = await gptExpertFileService.GetAsync(expertFile.Id);
        if(expertFileResult.Status == true)
        {
            var expertFileItem = expertFileResult.Payload!;
            expertFileItem.ProcessingStatus = CommonDomain.Enums.ExpertFileStatusEnum.ToEmbedding;
            await gptExpertFileService.UpdateAsync(expertFileItem);
        }
    }
}
