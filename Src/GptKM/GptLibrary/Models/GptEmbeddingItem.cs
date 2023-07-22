using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Models;

public class GptEmbeddingItem
{
    public string FileName { get; set; } = string.Empty;
    public int ChunkIndex { get; set; } = 0;
    public string ChunkContent { get; set; }
    public Vector<float> Embedding { get; set; } = Vector<float>.Build.Dense(0);
    public double CosineSimilarity { get; set; }
}
