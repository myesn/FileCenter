using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.FileCenter
{
    public class AddFileCommand : ICommand
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Extensions { get; set; }
        public long Length { get; set; }
    }
}
