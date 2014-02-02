using System;
using System.Collections.Generic;
using System.IO;
using Blacker.MangaReader.Collections;

namespace Blacker.MangaReader.ComicBook
{
    class ComicBook
    {
        private readonly ComicBookLoader _comicBookLoader;
        
        private AsyncObservableCollection<ComicBookPage> _pages;

        public ComicBook(string fileName)
        {
            _comicBookLoader = new ComicBookLoader();

            FileName = fileName;
            Name = (Path.GetFileNameWithoutExtension(fileName) ?? String.Empty).Replace("_", " ");
        }

        public string Name { get; private set; }
        public string FileName { get; private set; }

        public IEnumerable<ComicBookPage> Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new AsyncObservableCollection<ComicBookPage>();
                    _comicBookLoader.Load(FileName, _pages);
                }

                return _pages;
            }
        }
    }
}
