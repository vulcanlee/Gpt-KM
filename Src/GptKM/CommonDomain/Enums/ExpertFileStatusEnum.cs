using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommonDomain.Enums
{
    public enum ExpertFileStatusEnum
    {
        Begin,
        ToText,
        ToSplit,
        ToChunk,
        ToEmbedding,
        Finish,
    }
}
