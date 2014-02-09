using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Blacker.MangaReader.ComicBook
{
    class ComicBookPage
    {
        private readonly BitmapSource _originalImage;

        private BitmapSource _image;

        public ComicBookPage(string name, BitmapSource image, ComicBookPageType type)
        {
            if (image == null) 
                throw new ArgumentNullException("image");

            Name = name;
            _originalImage = image;
            PageType = type;

            PageIdentifier = Guid.NewGuid();
        }

        public Guid PageIdentifier { get; private set; }

        public string Name { get; private set; }

        public ComicBookPageType PageType { get; private set; }

        public BitmapSource Image
        {
            get { return _image ?? (_image = GetProcessedImage()); }
        }

        private BitmapSource GetProcessedImage()
        {
            switch (PageType)
            {
                case ComicBookPageType.Filler:
                case ComicBookPageType.WholePage:
                    return _originalImage;

                case ComicBookPageType.LeftHalf:
                    return new CroppedBitmap(_originalImage, new Int32Rect(0, 0, _originalImage.PixelWidth/2, _originalImage.PixelHeight));

                case ComicBookPageType.RightHalf:
                    return new CroppedBitmap(_originalImage,
                                             new Int32Rect(_originalImage.PixelWidth/2, 0, _originalImage.PixelWidth - _originalImage.PixelWidth/2, _originalImage.PixelHeight));

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
