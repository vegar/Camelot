using System.Windows.Input;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public abstract class FileSystemNodeViewModelBase : ViewModelBase, IFileSystemNodeViewModel
    {
        private readonly IFileSystemNodeOpeningBehavior _fileSystemNodeOpeningBehavior;
        private readonly IOperationsService _operationsService;

        private string _lastModifiedDateTime;
        private string _fullPath;
        private string _name;
        private string _fullName;
        private bool _isEditing;

        public string LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public string FullPath
        {
            get => _fullPath;
            set => this.RaiseAndSetIfChanged(ref _fullPath, value);
        }
        
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        public string FullName
        {
            get => _fullName;
            set => this.RaiseAndSetIfChanged(ref _fullName, value);
        }
        
        public bool IsEditing
        {
            get => _isEditing;
            set => this.RaiseAndSetIfChanged(ref _isEditing, value);
        }
        
        public bool IsSelected { get; set; }

        public ICommand OpenCommand { get; }
        
        public ICommand RenameCommand { get; }

        protected FileSystemNodeViewModelBase(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IOperationsService operationsService)
        {
            _fileSystemNodeOpeningBehavior = fileSystemNodeOpeningBehavior;
            _operationsService = operationsService;

            OpenCommand = ReactiveCommand.Create(Open);
            RenameCommand = ReactiveCommand.Create(Rename);
        }

        private void Open()
        {
            _fileSystemNodeOpeningBehavior.Open(FullPath);
        }
        
        private void Rename()
        {
            IsEditing = false;
            
            if (!string.IsNullOrEmpty(_fullName))
            {
                _operationsService.Rename(_fullPath, _fullName);
            }
        }
    }
}