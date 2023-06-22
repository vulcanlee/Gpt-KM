namespace GptLibrary.Models
{
    public class ExpertFileInfo
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Length { get; set; } = 0L;

        public ExpertFileInfo FromFileInfo(FileInfo fileInfo)
        {
            DirectoryName = fileInfo.DirectoryName;
            Extension = fileInfo.Extension;
            FullName = fileInfo.FullName;
            Name = fileInfo.Name;
            Length = fileInfo.Length;
            return this;
        }
    }
}