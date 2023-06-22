namespace GptLibrary.Models
{
    public class ExpertContent
    {
        public string TargetDirectory { get; set; }=string.Empty;
        public string ConvertDirectory { get; set; }=string.Empty;
        public List<ExpertRawFile> ExpertFiles { get; set; } = new List<ExpertRawFile>();
        public List<string> Extensions { get; set; } = new List<string>();
        public List<ExtensionSummary> ExtensionSummaries { get; set; } = new List<ExtensionSummary>();
    }
}