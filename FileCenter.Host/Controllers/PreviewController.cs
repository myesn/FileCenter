using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Upo.FileCenter.Host.Controllers
{
    [Route("[controller]")]
    public class PreviewController : Controller
    {
        private readonly FileCenterHostOptions _options;
        private readonly string _tempDirectory;
        private readonly string _physicalDirctory;
        private readonly FFMPEGHelper _ffmpegHelper;

        public PreviewController(IOptionsSnapshot<FileCenterHostOptions> options, FFMPEGHelper ffmpegHelper)
        {
            this._options = options.Value;
            this._tempDirectory = Path.Combine(this._options.UploadDirectory, "temps");
            this._physicalDirctory = Path.Combine(this._options.UploadDirectory, "physicals");
            this._ffmpegHelper = ffmpegHelper;
        }

        [HttpGet("Screenshot")]
        public IActionResult Screenshot()
        {
            var mp4Files = Directory.GetFiles(this._physicalDirctory, "*.mp4");
            var jpgFiles = Directory.GetFiles(this._physicalDirctory, "*.jpg");

            if (jpgFiles != null && jpgFiles.Any())
            {
                foreach (var jpgFile in jpgFiles)
                    System.IO.File.Delete(jpgFile);
            }

            foreach (var mp4File in mp4Files)
            {
                var outputThubPath = Path.Combine(Path.GetDirectoryName(mp4File), Path.GetFileNameWithoutExtension(mp4File) + ".jpg");

                this._ffmpegHelper.BuildCommand(mp4File, "5", "380", "260", outputThubPath).Screenshot();
            }

            return Ok("Screenshot done.");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var filesNames = Directory.GetFiles(this._physicalDirctory, "*.mp4").Select(x => Path.GetFileNameWithoutExtension(x));

            return Json(filesNames);
        }

        [HttpGet("screenshot/{filename}.mp4")]
        [ResponseCache(Duration = 2592000)]
        public async Task<IActionResult> Video(string filename)
        {
            var videoPath = Path.Combine(this._physicalDirctory, $"{filename}.mp4");
            if (!System.IO.File.Exists(videoPath))
                return Ok();

            return File(await System.IO.File.ReadAllBytesAsync(videoPath), "audio/mp4");
        }

        [HttpGet("screenshot/{filename}.jpg")]
        [ResponseCache(Duration = 2592000)]
        public async Task<IActionResult> Image(string filename)
        {
            var imagePath = Path.Combine(this._physicalDirctory, $"{filename}.jpg");
            if (!System.IO.File.Exists(imagePath))
                return Ok();

            return File(await System.IO.File.ReadAllBytesAsync(imagePath), "application/x-jpg");
        }

    }
}