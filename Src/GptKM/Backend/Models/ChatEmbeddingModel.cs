using Backend.Attributes.Validations;
using GptLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class ChatEmbeddingModel
    {
        public string Question { get; set; }=string.Empty;
        public string Answer { get; set; } = string.Empty;
        public bool DoSearching { get; set; } = false;
        public List<SearchResult> SearchResult { get; set; } = new List<SearchResult>();
    }

    public class SearchResult
    {
        public GptEmbeddingItem GptEmbeddingItem { get; set; } = new GptEmbeddingItem();
        public string CosineSimilarity { get; set; }
        public bool ShowEmbeddingText { get; set; } = false;
    }
}
