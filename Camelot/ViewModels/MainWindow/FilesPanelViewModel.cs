using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Factories.Interfaces;
using Camelot.Services.Interfaces;
using DynamicData;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FilesPanelViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IFilesSelectionService _filesSelectionService;
        private readonly IFileViewModelFactory _fileViewModelFactory;

        private readonly ObservableCollection<FileViewModel> _files;
        private readonly ObservableCollection<FileViewModel> _selectedFiles;

        private FileViewModel _selectedFile;
        private string _currentDirectory;

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => this.RaiseAndSetIfChanged(ref _currentDirectory, value);
        }

        public IEnumerable<FileViewModel> Files => _files;

        public IList<FileViewModel> SelectedFiles => _selectedFiles;

        public ICommand ActivateCommand { get; }

        public ICommand RefreshCommand { get; }

        public event EventHandler<EventArgs> ActivatedEvent;

        public FilesPanelViewModel(
            IFileService fileService,
            IDirectoryService directoryService,
            IFilesSelectionService filesSelectionService,
            IFileViewModelFactory fileViewModelFactory)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _filesSelectionService = filesSelectionService;
            _fileViewModelFactory = fileViewModelFactory;

            _files = new ObservableCollection<FileViewModel>();
            _selectedFiles = new ObservableCollection<FileViewModel>();
            _selectedFiles.CollectionChanged += SelectedFilesOnCollectionChanged;

            // TODO: load directory from settings by key/number
            CurrentDirectory = "/home/";
            ActivateCommand = ReactiveCommand.Create(Activate);
            RefreshCommand = ReactiveCommand.Create(ReloadFiles);

            this.WhenAnyValue(x => x.CurrentDirectory)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => ReloadFiles());

            ReloadFiles();
        }

        private void Activate()
        {
            ActivatedEvent.Raise(this, EventArgs.Empty);
        }

        private void ReloadFiles()
        {
            if (!_directoryService.DirectoryExists(CurrentDirectory))
            {
                return;
            }

            var directories = _directoryService.GetDirectories(CurrentDirectory);
            var files = _fileService.GetFiles(CurrentDirectory);

            var directoriesModels = directories
                .Select(_fileViewModelFactory.Create);
            var filesModels = files
                .Select(_fileViewModelFactory.Create);

            _files.Clear();
            _files.AddRange(directoriesModels.Concat(filesModels));
        }

        private void SelectedFilesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var filesToAdd = e.NewItems?
                .Cast<FileViewModel>()
                .Select(f => f.FullPath);
            if (filesToAdd != null)
            {
                _filesSelectionService.SelectFiles(filesToAdd);
            }

            var filesToRemove = e.OldItems?
                .Cast<FileViewModel>()
                .Select(f => f.FullPath);
            if (filesToRemove != null)
            {
                _filesSelectionService.SelectFiles(filesToRemove);
            }
        }
    }
}