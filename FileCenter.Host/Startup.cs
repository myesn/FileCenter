using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Upo.FileCenter.Host
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            this._env = env;
            this._configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FileCenterHostOptions>(_configuration.GetSection(nameof(FileCenterHostOptions)));
            services.AddSingleton<FFMPEGHelper>();

            services
                .AddFileCenterEntityFrameworkMySql(_configuration.GetConnectionString("FileDb"))
                .AddFileCenterProviders()
                .AddFileCenterCommandHandlers();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
            });
        }

        private async Task Echo(HttpContext context, WebSocket socket)
        {
            var uploadDirectory = Path.Combine(_env.WebRootPath, "uploads");
            var tempDirectory = Path.Combine(uploadDirectory, "temps");
            var physicalDirectory = Path.Combine(uploadDirectory, "physicals");

            EnsureDirecotrysExisting(uploadDirectory, tempDirectory, physicalDirectory);

            var buffer = new byte[1024 * 4];//4KB
            var bufferAll = default(byte[]);
            var fileName = default(string);
            var savePath = default(string);
            var index = 0;
            while (true)
            {
                var arraySegment = new ArraySegment<byte>(buffer);
                var result = await socket.ReceiveAsync(arraySegment, CancellationToken.None);
                if (result.CloseStatus.HasValue)
                    break;

                var currentLength = Math.Min(arraySegment.Count, result.Count);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var obj = JObject.Parse(Encoding.UTF8.GetString(arraySegment.ToArray(), 0, currentLength));
                    //开始上传
                    if (obj["msg"].ToString() == "begin")
                    {
                        // 发送完成，全部写入文件                        
                        fileName = $"{Guid.NewGuid().ToString("N")}.{obj["ext"].ToString()}";
                        savePath = Path.Combine(physicalDirectory, fileName);
                        bufferAll = new byte[int.Parse(obj["total"].ToString())];
                        tempDirectory = Path.Combine(tempDirectory, Guid.NewGuid().ToString("N"));

                        EnsureDirecotrysExisting(tempDirectory);
                    }
                    else//上传完成,合并文件
                    {
                        //await CombineMultipleFilesIntoSingleFile(tempDirectory, string.Empty, savePath);
                        await Merge(tempDirectory, string.Empty, savePath);

                        await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(fileName)), WebSocketMessageType.Text, true, default);

                        Directory.Delete(tempDirectory, true);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    if (currentLength == 0)
                        return;

                    using (var memoryStream = new MemoryStream(arraySegment.Take(currentLength).ToArray()))
                    using (var binaryReader = new BinaryReader(memoryStream))//, GetEncoding(memoryStream)
                    //using (var binaryWriter = new BinaryWriter(memoryStream,Encoding.UTF8))
                    {
                        var bytes = binaryReader.ReadBytes(currentLength);
                        var tempFilePath = Path.Combine(tempDirectory, (index++).ToString());
                        await SaveFile(tempFilePath, bytes);
                    }
                }
            }

            //await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private Encoding GetEncoding(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("require a stream that can seek");

            var encoding = Encoding.ASCII;
            var bom = new byte[5];
            var nRead = stream.Read(bom, offset: 0, count: bom.Length);
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
            {
                stream.Seek(4, SeekOrigin.Begin);
                return Encoding.UTF32;
            }
            else if (bom[0] == 0xff && bom[1] == 0xfe)
            {
                stream.Seek(2, SeekOrigin.Begin);
                return Encoding.Unicode;
            }
            else if (bom[0] == 0xfe && bom[1] == 0xff)
            {
                stream.Seek(2, SeekOrigin.Begin);
                return Encoding.BigEndianUnicode;
            }
            else if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            {
                stream.Seek(3, SeekOrigin.Begin);
                return Encoding.UTF8;
            }
            stream.Seek(0, SeekOrigin.Begin);
            return encoding;
        }


        private async Task SaveFile(string filePath, byte[] buffer)
        {
            try
            {
                using (var output = System.IO.File.Create(filePath))
                {
                    await output.WriteAsync(buffer, 0, buffer.Length);
                }
                //using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                //{
                //    await fs.WriteAsync(buffer, 0, buffer.Length);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task Merge(string inputDirectoryPath, string inputFileNamePattern, string outputFilePath)
        {
            string[] inputFilePaths = Directory.GetFiles(inputDirectoryPath, inputFileNamePattern);
            using (var outputStream = System.IO.File.Create(outputFilePath))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = System.IO.File.OpenRead(inputFilePath))
                    {
                        // Buffer size can be passed as the second argument.
                        await inputStream.CopyToAsync(outputStream);
                    }
                }
                await outputStream.FlushAsync();
            }
        }

        //private async Task Merge(string inputDirectoryPath, string inputFileNamePattern, string outputFilePath)
        //{
        //    string[] inputFilePaths = Directory.GetFiles(inputDirectoryPath, inputFileNamePattern);
        //    using (var output = File.Create(outputFilePath))
        //    {
        //        foreach (var inputFilePath in inputFilePaths)
        //        {
        //            using (var inputStream = File.OpenRead(inputFilePath))
        //            {
        //                var buffer = new byte[inputStream.Length];
        //                int bytesRead;
        //                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        //                {
        //                    await output.WriteAsync(buffer, 0, bytesRead);
        //                }
        //            }
        //        }
        //    }
        //}

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
