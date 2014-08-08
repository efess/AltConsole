using AltConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole
{
    public class InputOutputBufferHandler
    {
        public event ConsoleBufferUpdate OutputBufferUpdate;
        public event EventHandler<InputLineEventArgs> NewLine;
        public event EventHandler InputUpdate;
        public int CursorPosition {get; private set;}
        public string CurrentInput { get; private set; }
        public int CurrentBufferLineCount { get { return _outputBuffer.Count(); } }

        private delegate bool lineEndingChecker(char[] array, int index, out int newIndex);
        private List<char[]> _outputBuffer;

        private int _bufferCapacity;
        private char[] _lineEnding;

        private lineEndingChecker _lineEndingCheck;
        private Action<char[]> _addInput;

        public InputOutputBufferHandler(BufferProvider bufferProvider, InputHandler inputHandler)
        {
            inputHandler.InputChanged += (s, e) => ProcessInput(e.Input, e.CursorPosition);
            inputHandler.Command += (s, e) => OnNewLine(e.InputLine);
            bufferProvider.ProcessOut += (data) => AddOutput(data);
            _addInput = new Action<char[]>((chr) => bufferProvider.ProcessIn(chr));

            _outputBuffer = new List<char[]>();
            _lineEnding = Environment.NewLine.ToCharArray();
            _bufferCapacity = 1000;
            _lineEndingCheck = _lineEnding.Length == 1 // I really hope there are no three char line endings out there.......
                ? (lineEndingChecker)LineEndingOneChar
                : (lineEndingChecker)LineEndingTwoChars;
        }

        private void ProcessInput(string currentInput, int cursorPosition)
        {
            CurrentInput = currentInput;
            CursorPosition = cursorPosition;
            OnInputUpdate();
        }

        protected virtual void OnInputUpdate()
        {
            var evnt = InputUpdate;
            if(evnt != null)
            {
                evnt(this, new EventArgs());
            }
        }

        protected virtual void OnNewLine(string newLine)
        {
            var evnt = NewLine;
            if (evnt != null)
            {
                evnt(this, new InputLineEventArgs(newLine));
            }
        }

        public virtual void AddOutput(char[] line)
        {
            int offset = 0;
            int nextOffset = 0;
            int start = 0;

            while(offset < line.Length)
            {
                if(_lineEndingCheck(line, offset, out nextOffset))
                {
                    // at a newline
                    if(start > 0)
                    {
                        AddLineToOutputBuffer(line.Skip(start).Take(offset - start).ToArray());
                    }
                    else
                    {
                        AppendToCurrentLineInOutputBuffer(line.Skip(start).Take(offset - start).ToArray());
                    }
                    if (line.Length - offset == _lineEnding.Length) // the case where there is a newline at the end of the buffer
                    {
                        AddLineToOutputBuffer(new char[0]);
                    }

                    offset = nextOffset;
                    start = nextOffset;
                }
                else { offset++; }
            }


            if (start != offset)
            {
                if (start > 0)
                {
                    AddLineToOutputBuffer(line.Skip(start).Take(offset - start).ToArray());
                }
                else
                {
                    AppendToCurrentLineInOutputBuffer(line.Skip(start).Take(offset - start).ToArray());
                }
            }

            OnBufferUpdate(new ConsoleBufferUpdateEventArgs(ConsoleBufferUpdateType.Append, line));
        }

        public virtual char[][] GetOutputLines()
        {
            return _outputBuffer.ToArray();
        } 

        protected virtual void OnBufferUpdate(ConsoleBufferUpdateEventArgs bufferUpdate)
        {
            var bufferUpdateEvent = OutputBufferUpdate;
            if (bufferUpdateEvent != null)
            {
                bufferUpdateEvent(this, bufferUpdate);
            }
        }

        private void AppendToCurrentLineInOutputBuffer(char[] data)
        {
            if (_outputBuffer.Count == 0)
                _outputBuffer.Add(data);
            else
            {
                var currentLine = _outputBuffer[_outputBuffer.Count - 1];
                var newLine = new char[currentLine.Length + data.Length];

                Array.Copy(currentLine, newLine, currentLine.Length);
                Array.Copy(data, 0, newLine, currentLine.Length, data.Length);

                _outputBuffer[_outputBuffer.Count - 1] = newLine;
            }
        }

        private void AddLineToOutputBuffer(char[] line)
        {
            if (_outputBuffer.Count == _bufferCapacity) 
                _outputBuffer.RemoveAt(0);
            _outputBuffer.Add(line);
        }

        private bool LineEndingOneChar(char[] array, int index, out int newIndex)
        {
            if(array[index] == _lineEnding[0])
            {
                newIndex = index + 1;
                return true;
            }
            newIndex = index;
            return false;
        }
        private bool LineEndingTwoChars(char[] array, int index, out int newIndex)
        {
            if(array.Length - index > 1 
                && array[index] == _lineEnding[0] 
                && array[index + 1] == _lineEnding[1])
            {
                newIndex = index + 2;
                return true;
            }

            newIndex = index;
            return false;
        }
    }
}
