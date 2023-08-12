using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class ProductLicense
    {
        public string CompanyName { get; set; } = "公司名稱";
        public string ProductName { get; set; } = "智慧知識庫";
        public string Version { get; set; } = "版本";
        public string LicenseKey { get; set; } = string.Empty;
    }
}
