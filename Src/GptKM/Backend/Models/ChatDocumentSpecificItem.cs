using Domains.Models;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Models;

public class ChatDocumentSpecificItem
{
    public string FileName { get; set; } = string.Empty;
    public ExpertFile ExpertFile { get; set; }
    public ExpertFileChunk ExpertFileChunk { get; set; }
}
