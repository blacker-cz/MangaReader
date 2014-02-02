using System;

namespace Blacker.MangaReader.Utils
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            _value = value;
        }

        private readonly T _value;

        public T Value
        {
            get { return _value; }
        }
    }
}
