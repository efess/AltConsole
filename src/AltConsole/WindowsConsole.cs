using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using AltConsole.Interfaces;

namespace AltConsole
{
    // A lot of this could probably be "base classed"
    public class WindowsConsole :  IExternalProcess
    {
        private bool _processClosing;

        private const int _consoleOutSize = 100;

        private StreamWriter _consoleInStream;
        private StreamReader _consoleOutStream;
        private StreamReader _consoleErrStream;
        private Process _consoleProcess;
        
        private Thread _consoleErrReaderThread;
        private Thread _consoleOutReaderThread;
        private Thread _consoleOutPublisher;

        private int _bufferOffset;
        private char[] _consoleOutBuffer;
        private object _publishSynchObject;
        private object _bufferOutSynchObject;
        private long _lastUpdate;

        public WindowsConsole()
        {
            _bufferOutSynchObject = new object();
            _publishSynchObject = new object();
        }

        public event OutHandler ProcessOut;

        public void ProcessIn(char[] conChar)
        {
            if(_consoleInStream != null)
            {
                _consoleInStream.Write(conChar);
            }
        }

        public void Run()
        {
            _processClosing = false;
            _consoleOutPublisher = new Thread(PublishConsoleOut);
            _consoleOutPublisher.Start();
            _consoleOutReaderThread = new Thread(OutStreamBlockingReader);
            _consoleErrReaderThread = new Thread(ErrStreamBlockingReader);
            _consoleProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError= true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _consoleProcess.Exited += (s, e) => { _processClosing = true; };
            _consoleProcess.Start();
            
            _consoleInStream = _consoleProcess.StandardInput;
            _consoleOutStream = _consoleProcess.StandardOutput;
            _consoleErrStream = _consoleProcess.StandardError;
           
            _consoleOutReaderThread.Start();
            _consoleErrReaderThread.Start();
        }


        public void Stop()
        {
            _consoleProcess.Close();
            _processClosing = true;
            _consoleOutStream.Close();
            _consoleInStream.Close();
            _consoleErrStream.Close();
        }

        private void OutStreamBlockingReader()
        {
            while (!_processClosing)
            {
                char chr = (char)_consoleOutStream.Read();

                lock (_bufferOutSynchObject)
                {
                    while (_bufferOffset >= 100 || _consoleOutBuffer == null)
                    {
                        // wait for publish to write.
                        Thread.Sleep(10);
                    }

                    lock (_publishSynchObject)
                    {
                        _consoleOutBuffer[_bufferOffset++] = chr;
                    }
                }

            }
        }

        private void ErrStreamBlockingReader()
        {
            while (!_processClosing)
            {
                char chr = (char)_consoleErrStream.Read();

                lock (_bufferOutSynchObject)
                {
                    while (_bufferOffset >= 100 || _consoleOutBuffer == null)
                    {
                        // wait for publish to write.
                        Thread.Sleep(10);
                    }

                    lock (_publishSynchObject)
                    {
                        _consoleOutBuffer[_bufferOffset++] = chr;
                    }
                }

            }
        }

        protected virtual void PublishConsoleOut()
        {
            while (!_processClosing)
            {
                char[] charsToPublish = null;
                long currentTimeStamp = DateTime.Now.Ticks;
                lock (_publishSynchObject)
                {
                    if(_bufferOffset > 0)
                    {
                        if(_bufferOffset > _consoleOutBuffer.Length
                            || (currentTimeStamp  - _lastUpdate) > 10000)
                        {
                            charsToPublish = new char[_bufferOffset];
                            Array.Copy(_consoleOutBuffer, charsToPublish, charsToPublish.Length);
                            _bufferOffset = 0;
                            _consoleOutBuffer = new char[_consoleOutSize];
                        }
                    }
                    else if (_consoleOutBuffer == null)
                    {
                        _consoleOutBuffer = new char[_consoleOutSize];
                    }
                    else
                    {
                        _lastUpdate = currentTimeStamp;
                    }
                }

                if(charsToPublish != null)
                {
                    _lastUpdate = currentTimeStamp;
                    OnConsoleOut(charsToPublish);
                } 
                Thread.Sleep(10);
            }
        }

        protected virtual void OnConsoleOut(char[] conChar)
        {
            if(ProcessOut != null)
            {
                ProcessOut(conChar);
            }
        }
    }
}
