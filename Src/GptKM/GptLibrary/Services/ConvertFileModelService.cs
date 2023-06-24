﻿using EntityModel.Entities;
using GptLibrary.Models;
using Newtonsoft.Json;

namespace GptLibrary.Services;

/// <summary>
/// 將文字內容與切割後的文字Chunk，寫入到檔案內
/// </summary>
public class ConvertFileModelService
{
    /// <summary>
    /// 將指定的檔案名稱，把該檔案的文字內容轉換成為文字檔案
    /// </summary>
    /// <param name="expertFile"></param>
    /// <param name="convertFile"></param>
    /// <returns></returns>
    public async Task ExportConvertText(ExpertFile expertFile, ConvertFileModel convertFile)
    {
        string filename = convertFile.FileName;
        filename = filename.Replace(expertFile.ExpertDirectory.SourcePath, expertFile.ExpertDirectory.ConvertPath);
        await File.WriteAllTextAsync(filename, convertFile.SourceText);
    }

    /// <summary>
    /// 將指定 Chunk 的 字串內容 寫入到檔案內
    /// </summary>
    /// <param name="expertFile"></param>
    /// <param name="convertFile"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async Task ExportEmbeddingText(ExpertFile expertFile, ConvertFileModel convertFile, int index)
    {
        ConvertFileItemModel convertFileItemModel = convertFile.ConvertFileItems.FirstOrDefault(x => x.Index == index)!;
        string filename = convertFileItemModel.EmbeddingTextFileName;
        filename = filename.Replace(expertFile.ExpertDirectory.SourcePath, expertFile.ExpertDirectory.ConvertPath);
        await File.WriteAllTextAsync(filename, convertFileItemModel.SourceText);
    }

    /// <summary>
    /// 將指定 Chunk 的 Embedding 內容 寫入到檔案內
    /// </summary>
    /// <param name="expertFile"></param>
    /// <param name="convertFile"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async Task ExportEmbeddingJson(ExpertFile expertFile, ConvertFileModel convertFile, int index)
    {
        ConvertFileItemModel convertFileItemModel = convertFile.ConvertFileItems.FirstOrDefault(x => x.Index == index)!;
        string filename = convertFileItemModel.EmbeddingJsonFileName;
        filename = filename.Replace(expertFile.ExpertDirectory.SourcePath, expertFile.ExpertDirectory.ConvertPath);
        string content = JsonConvert.SerializeObject(convertFileItemModel.Embedding);
        await File.WriteAllTextAsync(filename, content);
    }

}
