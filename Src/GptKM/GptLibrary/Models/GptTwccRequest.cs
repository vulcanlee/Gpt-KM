namespace GptLibrary.Models;

public class GptTwccRequest
{
    public string model { get; set; } = "FFM-176B-latest";
    public string inputs { get; set; } = string.Empty;
    //public List<string> input { get; set; } = new();
}

public class GptTwccResponse
{
    public List<GptTwccResponseNode> Data { get; set; } = new();
}
public class GptTwccResponseNode
{
    public List<float> Embedding { get; set; } = new();
    public int index { get; set; }
}
