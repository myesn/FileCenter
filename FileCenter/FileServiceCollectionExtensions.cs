using System;
using System.Collections.Generic;
using System.Text;
using Upo.FileCenter;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileServiceCollectionExtensions
    {
        public static IServiceCollection AddFileEntityFrameworkMySql(this IServiceCollection services)
        {
            services.AddScoped<IFileProvider, FileProvider>();
            services.AddScoped<IFileMutableService, FileMutableService>();

            return services;
        }
    }
}
