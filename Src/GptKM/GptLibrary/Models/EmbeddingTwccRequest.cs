namespace GptLibrary.Models;

public class EmbeddingTwccRequest
{
    public List<string> input { get; set; } = new();
}

public class EmbeddingTwccResponse
{
    public List<EmbeddingTwccResponseNode> Data { get; set; } = new();
}
public class EmbeddingTwccResponseNode
{
    public List<float> Embedding { get; set; } = new();
    public int index { get; set; }
}
