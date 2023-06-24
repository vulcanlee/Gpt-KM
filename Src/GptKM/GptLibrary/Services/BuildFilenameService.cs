using GptLibrary.Models;

namespace GptLibrary.Services;

/// <summary>
/// 提供各種轉換 Embedding 會用到的檔案名稱
/// </summary>
public class BuildFilenameService
{
    public string BuildConvertedText(string fileName)
    {
        var newFilePath = $"{fileName}{GptConstant.ConvertToTextFileExtension}";
        return newFilePath;
    }
    public string BuildEmbeddingText(string fileName, int index)
    {
        var newFilePath = $"{fileName}.{index}{GptConstant.ConvertToEmbeddingTextFileExtension}";
        return newFilePath;
    }
    public string BuildEmbeddingJson(string fileName, int index)
    {
        var newFilePath = $"{fileName}.{index}{GptConstant.ConvertToEmbeddingJsonFileExtension}";
        return newFilePath;
    }
}
