using EntityModel.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.AdapterModels
{
    public class ExpertDirectoryAdapterModel : ICloneable
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "目錄對應的名稱 欄位必須要輸入值")]
        /// <summary>
        /// 這個目錄對應的名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "存在的目錄之絕對路徑名稱 欄位必須要輸入值")]
        /// <summary>
        /// 存在的目錄之絕對路徑名稱
        /// </summary>
        public string SourcePath { get; set; } = string.Empty;
        [Required(ErrorMessage = "轉換後的目錄之絕對路徑名稱 欄位必須要輸入值")]
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

        public virtual ICollection<ExpertFileAdapterModel> ExpertFile { get; set; }

        public ExpertDirectoryAdapterModel Clone()
        {
            return ((ICloneable)this).Clone() as ExpertDirectoryAdapterModel;
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
