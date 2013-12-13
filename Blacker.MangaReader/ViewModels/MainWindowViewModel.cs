using System.Collections.ObjectModel;

namespace Blacker.MangaReader.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            Pages = new ObservableCollection<string>
                        {
                            "page one",
                            "page two",
                            "page three",
                            "page four",
                            "page five"
                        };
        }

        public ObservableCollection<string> Pages { get; set; }
    }
}
