using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AltConsole.ViewModel
{
    public class FixedDimensionsDisplayViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private char[][] _displayBuffer;
        private InputOutputBufferHandler _bufferHandler;

        private IEnumerable<char[]> _display;
        private int _lines;
        private int _lineHeight;
        private int _linePosition;
        private int _scrollableLines;
        private int _screenWidth;
        private int _screenHeight;
        
        public int BufferLineCount { get { return _displayBuffer.Length; } }
        public int Lines { get { return _lines; } private set { if (_lines == value) return; _lines = value; OnUpdateDisplay(); } }
        public int Cols { get; private set; }

        public int LineHeight { get { return _lineHeight; } set { SetLineHeight(value); OnPropertyChanged("LineHeight"); } }
        public int Width { get { return _screenWidth; } set { SetScreenWidth(value); OnPropertyChanged("Width"); } }
        public int Height { get { return _screenHeight; } set { SetScreenHeight(value); OnPropertyChanged("Height"); } }
        public int ScrollableLines { get { return _scrollableLines; } set { _scrollableLines = value; OnPropertyChanged("ScrollableLines"); } }
        public IEnumerable<char[]> Display { get { return _display; } set { _display = value; OnPropertyChanged("Display"); } }
        public int LinePosition { 
            get { return _linePosition; } 
            set { 
                SetLinePosition(value);
                OnPropertyChanged("LinePosition"); } }

        // Commands
        // <none right now>


        public FixedDimensionsDisplayViewModel(InputOutputBufferHandler bufferHandler, int lines, int cols)
        {
            _lineHeight = 15;
            _lines = 0;
            //Lines = lines;
            Cols = cols;

            _bufferHandler = bufferHandler;

            _bufferHandler.OutputBufferUpdate += (s, e) => OnUpdateDisplay();
            _bufferHandler.InputUpdate += (s, e) => OnUpdateDisplay();
        }

        public void SetLinePosition(int newPosition)
        {
            if (_linePosition != newPosition
                && newPosition >= 0 && newPosition < _displayBuffer.Count())
            {
                _linePosition = newPosition;
                OnUpdateDisplay();
            }
        }

        private void CreateDisplayBuffer(char[][] currentBuffer)
        {
            var displayBuffer = new List<char[]>();
            var totalCounter = 0;
            if (currentBuffer.Length > 0)
                currentBuffer[currentBuffer.Length - 1] = (new string(currentBuffer[currentBuffer.Length - 1]) + _bufferHandler.CurrentInput).ToCharArray();

            for (int i = currentBuffer.GetUpperBound(0); i >= 0; i--)
            {
                var thisLine = currentBuffer[i];
                if (thisLine.Length > Cols)
                {
                    // cut line up
                    int lines = (int)Math.Ceiling((decimal)(thisLine.Length + 1) / (decimal)Cols);
                    int lineLength = thisLine.Length;
                    for (int j = lines - 1; j >= 0; j--)
                    {
                        int thisLength = j == lines - 1 && lineLength % Cols != 0 ? lineLength % Cols : Cols;

                        displayBuffer.Add(thisLine.Skip(Cols * j).Take(thisLength).ToArray());
                        lineLength -= thisLength;
                    }
                }
                else
                {
                    displayBuffer.Add(thisLine);
                }

                // Todo: do this smarter......
                //int lineWrapCount = (currentBuffer[i].Length + 1 / Cols);
                //if (currentBuffer[i].Length > Cols)

            }

            _displayBuffer = displayBuffer.ToArray();
        }

        protected virtual void SetScreenWidth(int newWidth)
        {
            _screenWidth = newWidth;
        }

        protected virtual void SetScreenHeight(int newHeight)
        {
            if (_screenHeight == newHeight)
            {
                return;
            }
            _screenHeight = newHeight;
            CalculateNewLineCount();
        }

        protected virtual void SetLineHeight(int newLineHeight)
        {
            if (_lineHeight == newLineHeight)
            {
                return;
            }
            _lineHeight = newLineHeight;
            CalculateNewLineCount();
        }

        protected virtual void CalculateNewLineCount()
        {
            if(_lineHeight <= 0)
                return;
            Lines = (int)Math.Floor((float)_screenHeight / (float)_lineHeight);
        }

        protected virtual void OnUpdateDisplay()
        {
            if (_bufferHandler == null)
                return;
            CreateDisplayBuffer(_bufferHandler.GetOutputLines());
            Display = _displayBuffer.Skip(LinePosition).Take(Lines).ToArray();
            ScrollableLines = Math.Max((_displayBuffer.Length - this.Lines), 0);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var evnt = PropertyChanged;
            if(evnt != null)
            {
                evnt(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
