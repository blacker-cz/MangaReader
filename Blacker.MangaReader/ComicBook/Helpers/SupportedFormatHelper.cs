using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Blacker.MangaReader.ComicBook.Helpers
{
    static class SupportedFormatHelper
    {
        private static readonly Regex SupportedExtensionsRegex = new Regex(@"\.(zip|rar|7z|tar|cbz|cbr|cb7|cbt)", RegexOptions.IgnoreCase);

        public static IEnumerable<string> SupportedExtensions
        {
            get
            {
                yield return "zip";
                yield return "rar";
                yield return "7z";
                yield return "tar";
                yield return "cbz";
                yield return "cbr";
                yield return "cb7";
                yield return "cbt";
            }
        }

        public static string DefaultExtension
        {
            get { return String.Concat(".", SupportedExtensions.First()); }
        }

        public static string OpenFileDialogFilter
        {
            get
            {
                return String.Format("Comic books|{0}", SupportedExtensions.Select(e => String.Concat("*.", e)).Aggregate((a, b) => String.Concat(a, ";", b)));
            }
        }

        public static bool IsSupported(string file)
        {
            if (String.IsNullOrEmpty(file))
                return false;

            return SupportedExtensionsRegex.IsMatch(Path.GetExtension(file) ?? String.Empty);
        }
    }
}
