using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole
{
    public struct KeyDown
    {
        public char Character;
        public System.Windows.Input.Key Key;
        public bool IsCtrl;
        public bool IsShift;
        public bool IsAlt;
        public bool IsWindows;
    }
}
