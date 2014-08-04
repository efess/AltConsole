using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole
{
    public class TestAsync
    {
        public void StartTest()
        {
            DoWork();
        }


        public async void DoWork()
        {
            await Ha();
        }

        
        public async Task Ha()
        {
            System.Threading.Thread.Sleep(500);
        }
    }
}
