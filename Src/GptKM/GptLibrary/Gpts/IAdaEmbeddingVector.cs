namespace GptLibrary.Gpts
{
    public interface IAdaEmbeddingVector
    {
        Task<float[]> GetEmbeddingAsync(string doc);
    }
}