using System;
using System.Text.RegularExpressions;

namespace Blacker.MangaReader.ComicBook.Helpers
{
    static class ComicPathFilterHelper
    {
        private static readonly Regex PathReplaceRegex = new Regex(Properties.Settings.Default.PathFilter.Replace(@"\\", @"\"), RegexOptions.IgnoreCase);

        public static string FilterPath(string path)
        {
            return String.IsNullOrEmpty(path) ? path : PathReplaceRegex.Replace(path, String.Empty);
        }
    }
}
