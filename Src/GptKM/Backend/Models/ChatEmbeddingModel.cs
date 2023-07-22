using Backend.Attributes.Validations;
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
    }
}
