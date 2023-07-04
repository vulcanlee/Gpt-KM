using CommonDomain.Enums;
using Domains.Models;

namespace Backend.AdapterModels
{
    public class ExpertFileAdapterModel : ICloneable
    {
        public int Id { get; set; }
        public int ExpertDirectoryId { get; set; }
        public ExpertDirectory ExpertDirectory { get; set; }
        /// <summary>
        /// 存在的目錄之絕對路徑名稱
        /// </summary>
        public string DirectoryName { get; set; } = string.Empty;
        /// <summary>
        /// 檔案名稱 - 含完整路徑名稱
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// 檔案名稱 - 不含路徑名稱
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// 副檔案名稱 - 含 句點 (.)
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        /// <summary>
        /// 檔案大小 - 單位: 位元組 (Byte) ，從檔案系統得到的數據
        /// </summary>
        public string ConvertDirectoryName { get; set; } = string.Empty;
        public long Size { get; set; } = 0L;
        public int TokenSize { get; set; } = 0;
        public decimal EmbeddingCost { get; set; } = 0;
        public bool ProcessChunk { get; set; } = false;
        public int ChunkSize { get; set; } = 0;
        public ExpertFileStatusEnum ProcessingStatus { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateAt { get; set; } = DateTime.Now;
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        /// <summary>
        /// 最後同步時間
        /// </summary>
        public DateTime SyncAt { get; set; } = DateTime.Now;

        public virtual ICollection<ExpertFileChunkAdapterModel> ExpertFileChunk { get; set; }

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
