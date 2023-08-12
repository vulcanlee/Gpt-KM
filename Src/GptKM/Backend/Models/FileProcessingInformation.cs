using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class FileProcessingInformation
    {
        public int 合計傳檔案數量 { get; set; }
        public long 合計尚未處理檔案數量 { get; set; }
        public long 合計檔案大小 { get; set; }
        public long 合計檔案區塊數量 { get; set; }
        public long 合計Token大小 { get; set; }
    }
}
