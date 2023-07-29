using Domains.Models;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Models;

public class GptEmbeddingCosineResultItem
{
    public GptEmbeddingItem GptEmbeddingItem { get; set; }
    public double CosineSimilarity { get; set; }
}
