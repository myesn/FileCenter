using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Upo.FileCenter.Host.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Upo.FileCenter.Host.Controllers
{
    [Route("[controller]")]
    public class FileController : Controller
    {
        private readonly FileCenterHostOptions _options;
        private readonly string _tempDirectory;
        private readonly string _physicalDirctory;

        public FileController(IOptionsSnapshot<FileCenterHostOptions> options)
        {
            this._options = options.Value;
            _tempDirectory = Path.Combine(this._options.UploadDirectory, "temps");
            _physicalDirctory = Path.Combine(this._options.UploadDirectory, "physicals");

            EnsureDirecotrysExisting(this._options.UploadDirectory, _tempDirectory, _physicalDirctory);
        }

        [HttpPost]
        public async Task<ActionResult> Upload([FromForm]UploadFileContext context)
        {
            var temporary = Path.Combine(_tempDirectory, context.LastModified);//临时保存分块的目录

            try
            {
                EnsureDirecotrysExisting(temporary);

                var filePath = Path.Combine(temporary, context.Index.ToString());
                if (!Convert.IsDBNull(context.Data))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await context.Data.CopyToAsync(fs);
                    }
                }

                var merged = false;
                if (context.Total == context.Index + 1)
                {
                    merged = await FileMerge(context.LastModified, context.FileName);
                }

                return Json(new
                {
                    context.Index,
                    merged
                });
            }
            catch (Exception ex)
            {
                Directory.Delete(temporary, true);//删除临时文件夹

                throw ex;
            }
        }

        public async Task<bool> FileMerge(string lastModified, string fileName)
        {
            var ok = false;
            try
            {
                var temporary = Path.Combine(_tempDirectory, lastModified);//临时文件夹
                var ext = Path.GetExtension(fileName);//获取文件后缀
                var files = Directory.GetFiles(temporary);//获得下面的所有文件
                var finalPath = Path.Combine(_physicalDirctory, DateTime.Now.ToString("yyMMddHHmmss") + ext);//最终的文件名（demo中保存的是它上传时候的文件名，实际操作肯定不能这样）
                using (var fs = new FileStream(finalPath, FileMode.Create))
                {
                    foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                    {
                        var bytes = System.IO.File.ReadAllBytes(part);

                        await fs.WriteAsync(bytes, 0, bytes.Length);

                        System.IO.File.Delete(part);//删除分块
                    }

                    Directory.Delete(temporary, true);//删除文件夹

                    ok = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ok;
        }

        private void EnsureDirecotrysExisting(params string[] directories)
        {
            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }
        }


    }
}