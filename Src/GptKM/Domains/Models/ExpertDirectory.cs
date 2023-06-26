using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.Entities;

/// <summary>
/// 生成語意內容的轉換目錄對應資訊
/// </summary>
public class ExpertDirectory
{
    public ExpertDirectory()
    {
    }

    public int Id { get; set; }
    /// <summary>
    /// 這個目錄對應的名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 存在的目錄之絕對路徑名稱
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;
    /// <summary>
    /// 轉換後的目錄之絕對路徑名稱 (存放 純文字內容、摘要、Chuck區塊文字)
    /// </summary>
    public string ConvertPath { get; set; } = string.Empty;
    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdateAt { get; set; } = DateTime.Now;

    public virtual ICollection<ExpertFile> ExpertFile { get; set; }
}
