using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.FileCenter
{
    public class File
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FileName { get; set; }
        public string ExtensionWithoutDot { get; set; }
        public string PhysicalPath { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public long DownloadTimes { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime UploadTime { get; set; } = DateTime.Now;
        public DateTime DeleteTime { get; set; } = DateTime.Now;

    }
}
