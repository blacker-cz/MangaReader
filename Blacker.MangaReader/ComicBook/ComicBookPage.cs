using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Blacker.MangaReader.ComicBook
{
    class ComicBookPage
    {
        public ComicBookPage(string name, BitmapSource image, ComicBookPageType type)
        {
            if (image == null) 
                throw new ArgumentNullException("image");

            Name = name;
            PageType = type;

            Image = ProcessImage(image, type);

            PageIdentifier = Guid.NewGuid();
        }

        public Guid PageIdentifier { get; private set; }

        public string Name { get; private set; }

        public ComicBookPageType PageType { get; private set; }

        public BitmapSource Image { get; private set; }

        private static BitmapSource ProcessImage(BitmapSource image, ComicBookPageType pageType)
        {
            switch (pageType)
            {
                case ComicBookPageType.Filler:
                case ComicBookPageType.WholePage:
                    return image;

                case ComicBookPageType.LeftHalf:
                    return new CroppedBitmap(image, new Int32Rect(0, 0, image.PixelWidth/2, image.PixelHeight));

                case ComicBookPageType.RightHalf:
                    return new CroppedBitmap(image, new Int32Rect(image.PixelWidth/2, 0, image.PixelWidth - image.PixelWidth/2, image.PixelHeight));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static ComicBookPage CreateFiller()
        {
            return new ComicBookPage(String.Empty, new BitmapImage(), ComicBookPageType.Filler);
        }
    }
}
