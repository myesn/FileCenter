﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Upo.FileCenter;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileProvidersServiceCollectionExtensions
    {
        public static IServiceCollection AddFileCenterProviders(
            this IServiceCollection services)
        {
            services.AddScoped<IFileProvider, FileProvider>();

            return services;
        }
    }
}
