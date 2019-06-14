using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Upo.FileCenter;
using Upo.FileCenter.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileEntityFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddFileCenterEntityFrameworkMySql(
            this IServiceCollection services,
            string sqlConnectionString)
        {
            services.AddDbContext<FileCenterDbContext>(builder => builder.UseMySql(sqlConnectionString));
            services.AddScoped<IStore, FileCenterDbContext>();

            return services;
        }
    }
}
