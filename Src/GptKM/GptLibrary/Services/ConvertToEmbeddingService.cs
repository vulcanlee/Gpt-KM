using EntityModel.Entities;
using GptLibrary.Converts;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Models;
using System.Dynamic;

namespace GptLibrary.Services
{
    public class ConvertToEmbeddingService
    {
        private readonly AdaEmbeddingVector adaEmbeddingVector;

        public ConvertToEmbeddingService(AdaEmbeddingVector adaEmbeddingVector)
        {
            this.adaEmbeddingVector = adaEmbeddingVector;
        }

        /// <summary>
        /// 將指定的檔案 Chunk，把文字內容轉換成為 Embedding
        /// </summary>
        /// <param name="expertFile"></param>
        public async Task Convert(ExpertFile expertFile, ConvertFileModel convertFile, int index)
        {
            string chunkembeddingFileName = Path
                            .Combine(expertFile.FullName, $"{expertFile.FileName}{GptConstant.ConvertToEmbeddingTextFileExtension}");
            string content =await File.ReadAllTextAsync(chunkembeddingFileName);
            float[] embeddings = await adaEmbeddingVector.GetEmbeddingAsync(content);
            ConvertFileItemModel convertFileItemModel = convertFile.ConvertFileItems.FirstOrDefault(x => x.Index == index)!;
            convertFileItemModel.Embedding = embeddings.ToList();
        }
    }
}
