namespace GptLibrary.Converts
{
    public interface IFileToText
    {
        Task<string> ToText(string filename);
    }
}