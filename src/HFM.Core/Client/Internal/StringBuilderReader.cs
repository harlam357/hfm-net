using System.Text;

namespace HFM.Core.Client.Internal
{
    // Shim to provide FahLogReader a TextReader implementation that reads content from a StringBuilder.
    [Serializable]
    internal class StringBuilderReader : TextReader
    {
        private StringBuilder _s;
        private int _pos;
        private int _length;

        public StringBuilderReader(StringBuilder s)
        {
            _s = s ?? throw new ArgumentNullException(nameof(s));
            _length = s.Length;
        }

        public override void Close() => Dispose(true);

        protected override void Dispose(bool disposing)
        {
            _s = null;
            _pos = 0;
            _length = 0;
            base.Dispose(disposing);
        }

        public override string ReadLine()
        {
            if (_s is null) throw new ObjectDisposedException(nameof(StringBuilderReader));

            int pos;
            for (pos = _pos; pos < _length; ++pos)
            {
                char ch = _s[pos];
                switch (ch)
                {
                    case '\n':
                    case '\r':
                        string str = _s.ToString(_pos, pos - _pos);
                        _pos = pos + 1;
                        if (ch == '\r' && _pos < _length && _s[_pos] == '\n')
                        {
                            ++_pos;
                        }
                        return str;
                    default:
                        continue;
                }
            }
            if (pos <= _pos)
            {
                return null;
            }
            string str1 = _s.ToString(_pos, pos - _pos);
            _pos = pos;
            return str1;
        }

        public override Task<string> ReadLineAsync() => Task.FromResult(ReadLine());
    }
}
