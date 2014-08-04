using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole.Interfaces
{
    public interface IInputHandler
    {
        event EventHandler<InputEventArgs> Input;
    }
}
