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
        public char InputCharacter { get; private set; }

        public InputEventArgs(char inputCharacter)
        {
            InputCharacter = inputCharacter;
        }
    }
}
