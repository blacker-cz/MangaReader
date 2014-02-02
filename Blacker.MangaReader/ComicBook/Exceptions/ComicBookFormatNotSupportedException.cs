using System;
using System.Runtime.Serialization;

namespace Blacker.MangaReader.ComicBook.Exceptions
{
    [Serializable]
    public class ComicBookFormatNotSupportedException : NotSupportedException
    {
        public string FileName { get; set; }

        public ComicBookFormatNotSupportedException(string fileName)
            : this(fileName, "Comic book format is not supported.")
        {
        }

        public ComicBookFormatNotSupportedException(string fileName, string message)
            : base(message)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            FileName = fileName;
        }

        public ComicBookFormatNotSupportedException(string fileName, string message, Exception inner)
            : base(message, inner)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            FileName = fileName;
        }

        protected ComicBookFormatNotSupportedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
