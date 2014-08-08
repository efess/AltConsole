using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole.Interfaces
{
    public interface IInputProvider
    {
        event EventHandler<InputKeyEventArgs> Input;
    }
}
