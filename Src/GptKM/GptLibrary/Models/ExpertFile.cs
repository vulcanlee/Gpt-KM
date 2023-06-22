namespace GptLibrary.Models
{
    public class ExpertRawFile
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public ExpertFileInfo FileInfo { get; set; }
        public string Extension { get; set; } = string.Empty;
        public long Size { get; set; } = 0L;
    }
}