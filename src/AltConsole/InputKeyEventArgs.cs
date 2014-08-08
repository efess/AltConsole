using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AltConsole
{
    public class InputKeyEventArgs
    {
        public KeyDown InputKey { get; private set; }

        public InputKeyEventArgs(KeyDown newKey)
        {
            InputKey = newKey;
        }
    }
}
