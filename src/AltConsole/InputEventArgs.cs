using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole
{
    public class InputEventArgs
    {
        // Maybe change this to something more intelligent at some point.
        public string Input { get; private set; }
        public int CursorPosition { get; private set; }

        public InputEventArgs(string input, int position)
        {
            Input = input;
            CursorPosition = position;
        }
    }
}
