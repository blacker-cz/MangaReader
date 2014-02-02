using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Blacker.MangaReader.Collections;
using Blacker.MangaReader.ComicBook.Exceptions;
using Blacker.MangaReader.ComicBook.Helpers;
using SharpCompress.Archive;
using log4net;

namespace Blacker.MangaReader.ComicBook
{
    class ComicBookLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(App));

        private static readonly Regex ImageExtensionsRegex = new Regex(@"\.(gif|png|jpg|jpeg|tif|tiff|bmp)", RegexOptions.IgnoreCase);

        private readonly BackgroundWorker _worker;
        private ICollection<ComicBookPage> _pages;
        private bool _loaded = false;

        public ComicBookLoader()
        {
            _worker = new BackgroundWorker
                          {
                              WorkerReportsProgress = true,
                              WorkerSupportsCancellation = true
                          };

            _worker.DoWork += WorkerOnDoWork;
            _worker.ProgressChanged += WorkerOnProgressChanged;
            _worker.RunWorkerCompleted += (sender, args) => { _loaded = true; };
        }

        public bool Loaded
        {
            get { return _loaded; }
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var file = e.Argument as string;
            var backgroundWorker = sender as BackgroundWorker;

            if (file == null || backgroundWorker == null)
                return;

            try
            {
                using (var stream = File.OpenRead(file))
                {
                    using (var archive = ArchiveFactory.Open(stream))
                    {
                        var entries = archive.Entries.Where(entry => !entry.IsDirectory)
                                             .OrderByAlphaNumeric(entry => entry.FilePath)
                                             .Where(entry => !entry.IsDirectory && IsImage(entry.FilePath))
                                             .ToList();

                        int counter = 0;

                        foreach (var entry in entries)
                        {
                            using (var entryStream = entry.OpenEntryStream())
                            {
                                var memoryStream = new MemoryStream();
                                CopyStream(memoryStream, entryStream);
                                memoryStream.Seek(0, SeekOrigin.Begin);

                                var progressParams = new ProgressParams(entry.FilePath, memoryStream);

                                backgroundWorker.ReportProgress(++counter, progressParams);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Unable to process file: " + file, ex);
            }
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressParams = e.UserState as ProgressParams;
            if (progressParams == null) 
                return;

            try
            {
                _log.Debug("Processing page: " + progressParams.Name);

                using (var stream = progressParams.Stream)
                {
                    var image = new BitmapImage();

                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    if (image.PixelWidth >= image.PixelHeight)
                    {
                        if (_pages.Count%2 != 1)
                            _pages.Add(new ComicBookPage(String.Empty, new BitmapImage(), ComicBookPageType.WholePage));

                        _pages.Add(new ComicBookPage(progressParams.Name, image, ComicBookPageType.LeftHalf));
                        _pages.Add(new ComicBookPage(progressParams.Name, image, ComicBookPageType.RightHalf));
                    }
                    else
                    {
                        _pages.Add(new ComicBookPage(progressParams.Name, image, ComicBookPageType.WholePage));
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Unable to load page.", ex);
            }
        }

        /// <summary>
        /// Load comic book from file.
        /// </summary>
        /// <param name="file">Full path to comic book.</param>
        /// <param name="pages">Collection to which all the pages will be loaded.</param>
        public void Load(string file, ICollection<ComicBookPage> pages)
        {
            if (file == null) 
                throw new ArgumentNullException("file");
            if (pages == null) 
                throw new ArgumentNullException("pages");

            if (!File.Exists(file))
                throw new FileNotFoundException();

            if (!SupportedFormatHelper.IsSupported(file))
                throw new ComicBookFormatNotSupportedException(file);

            if (_worker.IsBusy)
                throw new InvalidOperationException();

            _loaded = false;
            _pages = pages;
            _worker.RunWorkerAsync(file);
        }

        private static bool IsImage(string file)
        {
            return !String.IsNullOrEmpty(file) && ImageExtensionsRegex.IsMatch(Path.GetExtension(file) ?? String.Empty);
        }

        [Obsolete("This method is here only until next version of SharpCompress (next one after 0.10.3.0) where Can* methods are fixed")]
        private static void CopyStream(Stream output, Stream input)
        {
            try
            {
                input.CopyTo(output);
            }
            catch (NotImplementedException)
            {
                var buffer = new byte[4096];
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) != 0)
                {
                    output.Write(buffer, 0, read);
                }
            }
        }

        private class ProgressParams
        {
            public ProgressParams(string name, MemoryStream stream)
            {
                Name = name;
                Stream = stream;
            }

            public string Name { get; private set; }

            public MemoryStream Stream { get; private set; }
        }
    }
}
