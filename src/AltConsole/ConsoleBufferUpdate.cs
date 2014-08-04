using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltConsole
{
    public enum ConsoleBufferUpdateType
    {
        Append,
        Clear
    }

    public class ConsoleBufferUpdateEventArgs
    {
        public ConsoleBufferUpdateEventArgs(ConsoleBufferUpdateType updateType, char[] updateData)
        {
            UpdateType = updateType;
            UpdateData = updateData;
        }

        public ConsoleBufferUpdateType UpdateType { get; private set; }
        public char[] UpdateData { get; private set; }
    
    }
    public delegate void ConsoleBufferUpdate(object sender, ConsoleBufferUpdateEventArgs e);
}
