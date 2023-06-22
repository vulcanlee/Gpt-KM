using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.Entities;

public class ExpertDirectory
{
    public ExpertDirectory()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 存在的目錄之絕對路徑名稱
    /// </summary>
    public string Path { get; set; } = string.Empty;
    /// <summary>
    /// 轉換後的目錄之絕對路徑名稱 (存放 純文字內容、摘要、Chuck區塊文字)
    /// </summary>
    public string ConvertPath { get; set; } = string.Empty;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public DateTime? UpdateAt { get; set; } = DateTime.Now;
}
