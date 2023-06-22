﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.Entities;

public class ExpertFile
{
    public ExpertFile()
    {
    }

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
    public bool ProcessChunk { get; set; } =false;
    public int ChunkSize { get; set; } = 0;

}
