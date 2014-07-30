using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blacker.MangaReader.ComicBook.Exceptions;
using Blacker.MangaReader.ComicBook.Helpers;
using Blacker.MangaReader.Collections;
using Blacker.MangaReader.Utils;

namespace Blacker.MangaReader.ComicBook
{
    class ComicBookManager
    {
        private readonly Cache<string,IEnumerable<string>> _filesCache = new Cache<string, IEnumerable<string>>(TimeSpan.FromMinutes(1));

        public ComicBook Open(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentException("Path to file must not be null or empty.", "file");

            if (!File.Exists(file))
                throw new FileNotFoundException();

            if (!SupportedFormatHelper.IsSupported(file))
                throw new ComicBookFormatNotSupportedException(file);

            return new ComicBook(file);
        }

        public ComicBook OpenNext(ComicBook comicBook)
        {
            if (comicBook == null) 
                throw new ArgumentNullException("comicBook");

            var next = ListFiles(comicBook)
                .SkipWhile(f => !f.Equals(comicBook.FileName, StringComparison.OrdinalIgnoreCase))
                .Skip(1)
                .FirstOrDefault();

            if (next == null)
                return null;

            return new ComicBook(next);
        }

        public ComicBook OpenPrevious(ComicBook comicBook)
        {
            if (comicBook == null) 
                throw new ArgumentNullException("comicBook");

            var previous = ListFiles(comicBook)
                .TakeWhile(f => !f.Equals(comicBook.FileName, StringComparison.OrdinalIgnoreCase))
                .LastOrDefault();

            if (previous == null)
                return null;

            return new ComicBook(previous);
        }

        public IEnumerable<string> ListFiles(ComicBook comicBook)
        {
            if (comicBook == null) 
                throw new ArgumentNullException("comicBook");

            return ListFiles(Path.GetDirectoryName(comicBook.FileName));
        }

        public IEnumerable<string> ListFiles(string path)
        {
            if (!Directory.Exists(path))
                return Enumerable.Empty<string>();

            var files = _filesCache[path];

            if (files != null)
                return files;

            files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Where(SupportedFormatHelper.IsSupported).OrderByAlphaNumeric().ToArray();

            _filesCache[path] = files;

            return files;
        }
    }
}
