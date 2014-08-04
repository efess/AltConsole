using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole
{
    public class InputLineEventArgs
    {
        public string InputLine { get; private set;}
        public InputLineEventArgs(string newLine)
        {
            InputLine = newLine;
        }
    }
}
