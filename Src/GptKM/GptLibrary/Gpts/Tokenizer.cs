
using SharpToken;

namespace GptLibrary.Gpt;

/// <summary>
/// 將文字內容轉換成為 GPT-3 所需的 token 數量
/// </summary>
public class Tokenizer
{
    /// <summary>
    /// 計算所指定的文字內容，需要用到多少的 Token
    /// </summary>
    /// <param name="content">文字內容</param>
    /// <returns></returns>
    public int CountToken(string content)
    {
        var encoding = GptEncoding.GetEncoding("cl100k_base");
        List<int> tokens = encoding.Encode(content);
        return tokens.Count;
    }
}
