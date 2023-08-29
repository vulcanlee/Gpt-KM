namespace GptLibrary.Models;

public class GptTwccRequest
{
    public string model { get; set; } = "FFM-176B-latest";
    public string inputs { get; set; } = string.Empty;
    //public List<string> input { get; set; } = new();
}

public class GptTwccResponse
{
    public string generated_text { get; set; }=string.Empty;
    public string total_time_taken { get; set; }=string.Empty;
    public int generated_tokens { get; set; }
}
