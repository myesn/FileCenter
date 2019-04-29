using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.FileCenter.EntityFrameworkCore
{
    public class FileCenterDbContext : DbContext, IStore
    {
        public FileCenterDbContext(DbContextOptions<FileCenterDbContext> options) : base(options)
        {
            this.DbContext = this;
        }
        public DbSet<File> Files { get; set; }
        public DbContext DbContext { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>(entity =>
            {
                entity.ToTable($"{nameof(File)}s");

                entity.HasKey(x => x.Id);
            });
        }
    }
}
