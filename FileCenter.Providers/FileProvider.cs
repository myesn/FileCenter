using System;
using System.Collections.Generic;
using System.Text;
using Upo.CRUD.BasicProvider;

namespace Upo.FileCenter
{
    public class FileProvider : BasicProvider<File>, IFileProvider
    {
        public FileProvider(IStore store) : base(store)
        {
        }
    }
}
