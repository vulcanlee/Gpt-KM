namespace GptLibrary.Services;

public class ConvertFileExtensionMatchService
{
    List<string> canConvertFileExtensions;
    public ConvertFileExtensionMatchService()
    {
        canConvertFileExtensions = new List<string>()
        {
            ".pdf",
            ".html",
            ".xlsx",
            ".docx",
            ".txt",
            ".pptx",
            ".md"
        };
    }
    public bool IsMatch(string fileName)
    {
        var extension = System.IO.Path.GetExtension(fileName).ToLower();
        if (canConvertFileExtensions.Contains(extension))
        {
            return true;
        }
        return false;
    }
}
