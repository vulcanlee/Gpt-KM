using GptLibrary.Models;

namespace GptLibrary.Services;

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
