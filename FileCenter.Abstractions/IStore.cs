using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Upo.CRUD;

namespace Upo.FileCenter
{
    public interface IStore : IBasicStore
    {
        DbSet<File> Files { get; set; }
    }
}
