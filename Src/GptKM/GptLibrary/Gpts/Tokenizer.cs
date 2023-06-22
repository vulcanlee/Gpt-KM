using AI.Dev.OpenAI.GPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Gpt
{
    public class Tokenizer
    {
        public int CountToken(string content)
        {
            List<int> tokens = GPT3Tokenizer.Encode(content);
            return tokens.Count;

        }
    }
}
