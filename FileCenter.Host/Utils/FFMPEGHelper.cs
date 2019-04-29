using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upo.FileCenter.Host
{
    public class FFMPEGHelper
    {
        public FFMPEGHelper()
            : this("ffmpeg")
        {

        }

        public FFMPEGHelper(string ffmpegPath)
        {
            this._ffmpegPath = ffmpegPath;
        }

        private readonly string _ffmpegPath;
        private string _originVideoPath;
        private string _frameIndex;
        private string _thubWidth;
        private string _thubHeight;
        private string _thubImagePath;

        /// <summary>
        /// 构建命令
        /// </summary>
        /// <param name="originVideoPath">视频路径</param>
        /// <param name="frameIndex">截取帧所在的秒数</param>
        /// <param name="thubWidth">缩略图的宽度</param>
        /// <param name="thubHeight">缩略图的高度</param>
        /// <param name="thubImagePath">缩略图输出路径</param>
        public FFMPEGHelper BuildCommand(
            string originVideoPath,
            string frameIndex,
            string thubWidth,
            string thubHeight,
            string thubImagePath
            )
        {
            this._originVideoPath = originVideoPath;
            this._frameIndex = frameIndex;
            this._thubWidth = thubWidth;
            this._thubHeight = thubHeight;
            this._thubImagePath = thubImagePath;

            return this;
        }

        public string Screenshot()
        {
            var arguments = string.Format("-i \"{0}\" -ss {1} -vframes 1 -r 1 -ac 1 -ab 2 -s {2}*{3} -f image2 \"{4}\" -n",
                _originVideoPath,
                _frameIndex,
                _thubWidth,
                _thubHeight,
                _thubImagePath);

            Process.Start(new ProcessStartInfo(_ffmpegPath)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments
            });

            return _thubImagePath;
        }

    }
}
