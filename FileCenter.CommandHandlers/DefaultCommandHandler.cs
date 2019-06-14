using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Upo.FileCenter;

namespace FileCenter.CommandHandlers
{
    public class DefaultCommandHandler : ICommandHandler
    {
        private readonly IStore _store;

        public DefaultCommandHandler(IStore store)
        {
            _store = store;
        }

        public Task ExecuteAsync(ICommand command)
        {
            switch (command)
            {
                case AddFileCommand addFileCommand:
                    return ExecuteAsync(addFileCommand);
                default:
                    throw new NotSupportedException(command.GetType().ToString());
            }
        }

        private async Task ExecuteAsync(AddFileCommand command)
        {
            var file = new File();

            file.Id = command.Id;
            file.UserId = command.UserId;
            file.FileName = command.Name;
            file.Length = command.Length;
            file.Extensions = command.Extensions;

            _store.Files.Add(file);
            await _store.SaveChangesAsync();
        }
    }
}
