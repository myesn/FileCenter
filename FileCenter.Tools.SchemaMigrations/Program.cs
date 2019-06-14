using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using Upo.FileCenter.EntityFrameworkCore;

namespace FileCenter.Tools.SchemaMigrations
{
    class Program : IDesignTimeDbContextFactory<FileCenterDbContext>
    {
        private const string _feedDbConnectionString = "server=bsyan.cn;database=File;uid=root;pwd=!FlVpw2Qv75Wdem0cE@2;";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public FileCenterDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FileCenterDbContext>()
             .UseMySql(_feedDbConnectionString, builder =>
                builder.MigrationsAssembly(typeof(Program).Namespace));

            return new FileCenterDbContext(optionsBuilder.Options);
        }
    }
}
