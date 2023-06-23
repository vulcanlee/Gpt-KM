namespace GptLibrary.Services;

public class BuildFilenameService
{
    public string BuildConvertedText(string fileName)
    {
        var newFilePath = $"{fileName}.ConvertedText";
        return newFilePath;
    }
    public string BuildEmbeddingText(string fileName, int index)
    {
        var newFilePath = $"{fileName}.{index}.EmbeddingText";
        return newFilePath;
    }
    public string BuildEmbeddingJson(string fileName, int index)
    {
        var newFilePath = $"{fileName}.{index}.EmbeddingJson";
        return newFilePath;
    }
}
