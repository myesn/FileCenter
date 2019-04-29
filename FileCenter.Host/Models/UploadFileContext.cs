using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upo.FileCenter.Host.Models
{
    public class UploadFileContext
    {
        public IFormFile Data { get; set; }
        public string LastModified { get; set; }
        public int Total { get; set; }
        public string FileName { get; set; }
        public int Index { get; set; }
    }
}
