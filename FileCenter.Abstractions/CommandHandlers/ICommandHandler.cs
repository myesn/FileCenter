using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.FileCenter
{
    public interface ICommandHandler
    {
        Task ExecuteAsync(ICommand command);
    }
}
