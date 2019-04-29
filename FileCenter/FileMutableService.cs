using System;
using System.Collections.Generic;
using System.Text;
using Upo.CRUD.BasicMutableService;

namespace Upo.FileCenter
{
    public class FileMutableService : MutableBasicService<File>, IFileMutableService
    {
        public FileMutableService(IStore store) : base(store)
        {
        }
    }
}
