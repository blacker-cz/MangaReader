using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Blacker.MangaReader.Converters
{
    internal class PathToChapterNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string) value;

            var fileName = Path.GetFileNameWithoutExtension(path);

            return (fileName ?? "Unknown").Replace("_", " ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
