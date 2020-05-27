using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class DeleteFileOperation : OperationBase, IInternalOperation
    {
        private readonly string _fileToRemove;
        private readonly IFileService _fileService;

        public DeleteFileOperation(
            string fileToRemove,
            IFileService fileService)
        {
            _fileToRemove = fileToRemove;
            _fileService = fileService;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                OperationState = OperationState.InProgress;
                _fileService.Remove(_fileToRemove);
                OperationState = OperationState.Finished;
            }
            catch
            {
                // TODO: process exception
                OperationState = OperationState.Cancelled;
            }

            return Task.CompletedTask;
        }
    }
}