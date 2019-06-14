using FileCenter.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using Upo.FileCenter;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileCommandHandlersServiceCollectionExtensions
    {
        public static IServiceCollection AddFileCenterCommandHandlers(
            this IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, DefaultCommandHandler>();

            return services;
        }
    }
}
