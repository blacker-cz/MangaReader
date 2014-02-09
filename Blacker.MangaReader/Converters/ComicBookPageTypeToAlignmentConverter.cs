using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Blacker.MangaReader.ComicBook;

namespace Blacker.MangaReader.Converters
{
    internal class ComicBookPageTypeToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pageType = (ComicBookPageType) value;

            switch (pageType)
            {
                case ComicBookPageType.WholePage:
                case ComicBookPageType.Filler:
                    return HorizontalAlignment.Center;

                case ComicBookPageType.LeftHalf:
                    return HorizontalAlignment.Right;

                case ComicBookPageType.RightHalf:
                    return HorizontalAlignment.Left;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
