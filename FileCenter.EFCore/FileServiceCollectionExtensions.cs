using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Upo.FileCenter;
using Upo.FileCenter.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileServiceCollectionExtensions
    {
        public static IServiceCollection AddFileEntityFrameworkMySql(
            this IServiceCollection services,
            string sqlConnectionString,
            string migrationAssemblyName)
        {
            services.AddDbContext<FileCenterDbContext>(builder =>
                builder.UseMySql(sqlConnectionString, options => 
                    options.MigrationsAssembly(migrationAssemblyName)));
            services.AddScoped<IStore, FileCenterDbContext>();

            return services;
        }
    }
}
