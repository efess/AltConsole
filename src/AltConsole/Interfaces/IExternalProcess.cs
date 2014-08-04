using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltConsole.Interfaces
{
    public interface IExternalProcess
    {
        void Run();
        void Stop();
        event OutHandler ProcessOut;
        void ProcessIn(char[] conChar);
    }
}
