using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using Blacker.MangaReader.ComicBook;
using Blacker.MangaReader.ComicBook.Helpers;
using Blacker.MangaReader.Commands;
using Blacker.MangaReader.Collections;
using Blacker.MangaReader.Services;
using Blacker.MangaReader.Utils;
using System.Linq;

namespace Blacker.MangaReader.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private readonly IInteractionService _interactionService;
        private readonly ICommand _pageClickedCommand;
        private readonly ICommand _nextPageCommand;
        private readonly ICommand _prevPageCommand;

        private readonly ICommand _prevChapterCommand;
        private readonly ICommand _nextChapterCommand;

        private readonly ICommand _openChapterCommand;

        private readonly ComicBookManager _comicBookManager;
        private ComicBook.ComicBook _currentComicBook;

        private int _currentSheetIndex;

        public event EventHandler<EventArgs<TurnPageRequestType>> TurnPageRequested;

        public MainWindowViewModel(IInteractionService interactionService)
        {
            if (interactionService == null) 
                throw new ArgumentNullException("interactionService");

            _interactionService = interactionService;
            _comicBookManager = new ComicBookManager();

            var arguments = Environment.GetCommandLineArgs();
            if (arguments.Count() > 1)
            {
                CurrentComicBook = _comicBookManager.Open(arguments.Skip(1).First());
            }

            _pageClickedCommand = new RelayCommand(PageClicked);
            _nextPageCommand = new RelayCommand(GoToNextPage);
            _prevPageCommand = new RelayCommand(GoToPreviousPage);

            _prevChapterCommand = new RelayCommand(GoToPreviousChapter, o => CurrentComicBook != null);
            _nextChapterCommand = new RelayCommand(GoToNextChapter, o => CurrentComicBook != null);

            _openChapterCommand = new RelayCommand(OpenChapter);
        }

        public int CurrentSheetIndex
        {
            get { return _currentSheetIndex; }
            set
            {
                if (_currentSheetIndex == value) 
                    return;

                _currentSheetIndex = value;
                OnPropertyChanged(() => CurrentSheetIndex);
            }
        }

        public IEnumerable<ComicBookPage> Pages { get { return CurrentComicBook == null ? Enumerable.Empty<ComicBookPage>() : CurrentComicBook.Pages; } }

        public ICommand PageClickedCommand { get { return _pageClickedCommand; } }

        public ICommand NextPageCommand { get { return _nextPageCommand; } }

        public ICommand PrevPageCommand { get { return _prevPageCommand; } }

        public ICommand PrevChapterCommand { get { return _prevChapterCommand; } }

        public ICommand NextChapterCommand { get { return _nextChapterCommand; } }

        public ICommand OpenChapterCommand { get { return _openChapterCommand; } }

        public ComicBook.ComicBook CurrentComicBook
        {
            get { return _currentComicBook; }
            set
            {
                if(value == null)
                    return;

                _currentComicBook = value;
                OnPropertyChanged(() => Pages);
                OnPropertyChanged(() => CurrentComicBookName);
            }
        }

        public string CurrentComicBookName
        {
            get { return _currentComicBook == null ? String.Empty : _currentComicBook.Name; }
        }

        private void PageClicked(object param)
        {
            var page = param as ComicBookPage;

            if(page == null)
                return;

            int index = Pages.FindIndex(x => x.PageIdentifier == page.PageIdentifier);

            if(index == -1)
                return;

            OnTurnPageRequested(index%2 == 1 ? TurnPageRequestType.Previous : TurnPageRequestType.Next);
        }

        private void GoToNextPage(object param)
        {
            OnTurnPageRequested(TurnPageRequestType.Next);
        }

        private void GoToPreviousPage(object param)
        {
            OnTurnPageRequested(TurnPageRequestType.Previous);
        }

        private void GoToNextChapter(object param)
        {
            if(CurrentComicBook == null)
                return;

            var chapter = _comicBookManager.OpenNext(CurrentComicBook);
            if (chapter != null)
            {
                CurrentComicBook = chapter;
            }
        }

        private void GoToPreviousChapter(object param)
        {
            if (CurrentComicBook == null)
                return;

            var chapter = _comicBookManager.OpenPrevious(CurrentComicBook);
            if (chapter != null)
            {
                CurrentComicBook = chapter;
            }
        }

        private void OpenChapter(object param)
        {
            _interactionService.ShowOpenFileDialog(SupportedFormatHelper.DefaultExtension, SupportedFormatHelper.OpenFileDialogFilter,
                                                   (result, path) =>
                                                       {
                                                           if (result == DialogResult.OK)
                                                           {
                                                               var chapter = _comicBookManager.Open(path);
                                                               if (chapter != null)
                                                               {
                                                                   CurrentComicBook = chapter;
                                                               }
                                                           }
                                                       });
        }

        private void OnTurnPageRequested(TurnPageRequestType requestType)
        {
            var turnPageRequest = TurnPageRequested;
            if (turnPageRequest != null)
            {
                turnPageRequest(this, new EventArgs<TurnPageRequestType>(requestType));
            }
        }
    }
}
