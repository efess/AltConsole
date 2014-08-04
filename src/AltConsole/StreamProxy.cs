using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AltConsole
{
    public class StreamReaderProxy<T> : StreamReader where T : StreamReader 
    {
        
        private T _baseStream;
        public StreamReaderProxy(T baseStream)
            : base(baseStream.BaseStream)
        {
            _baseStream = baseStream;
        }

        public override int Read()
        {
            return base.Read();
        }
    }
}
