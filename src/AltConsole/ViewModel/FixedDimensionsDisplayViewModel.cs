using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        private int _chars;
        private int _lines;
        
        private SizeF _characterSize;
        private int _linePosition;
        private int _scrollableLines;
        private int _screenWidth;
        private int _screenHeight;
        private int _caretPosition;
        
        public int BufferLineCount { get { return _displayBuffer.Length; } }

        // Number of displayable lines (Screen Height)
        public int Lines { get { return _lines; } private set { if (_lines == value) return; _lines = value; OnUpdateDisplay(); } }
        // Number of displable character (Screen Width)
        public int Cols { get { return _chars; } private set { if (_chars == value) return; _chars = value; OnUpdateDisplay(); } }
        public SizeF CharacterSize { get { return _characterSize; } private set { if (_characterSize == value) return; _characterSize = value; CalculateNewDisplayDimensions(); } }
        //public int LineHeight { get { return _lineHeight; } set { SetLineHeight(value); OnPropertyChanged("LineHeight"); } }
        public int Width { get { return _screenWidth; } set { SetScreenWidth(value); OnPropertyChanged("Width"); } }
        public int Height { get { return _screenHeight; } set { SetScreenHeight(value); OnPropertyChanged("Height"); } }
        // Number of lines that are scrollable
        public int ScrollableLines { get { return _scrollableLines; } set { _scrollableLines = value; OnPropertyChanged("ScrollableLines"); } }
        public IEnumerable<char[]> Display { get { return _display; } set { _display = value; OnPropertyChanged("Display"); } }
        public int CaretPosition { get { return _caretPosition; } set {
            if (_caretPosition != value)
            {
                _caretPosition = value;
                OnPropertyChanged("CaretPosition");
            }
        } }
        public int LinePosition { 
            get { return _linePosition; } 
            set { 
                SetLinePosition(value);
                OnPropertyChanged("LinePosition"); } }

        // Commands
        // <none right now>


        public FixedDimensionsDisplayViewModel(InputOutputBufferHandler bufferHandler, int lines, int cols)
        {
            _lines = 0;
            //Lines = lines;
            //Cols = cols;

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
            if (Lines <= 0 || Cols <= 0) // Can only display if we have space...
                return;
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

                CaretPosition = _bufferHandler.CursorPosition;
                // Todo: do this smarter......
                //int lineWrapCount = (currentBuffer[i].Length + 1 / Cols);
                //if (currentBuffer[i].Length > Cols)

            }

            _displayBuffer = displayBuffer.ToArray();
        }

        protected virtual void SetScreenWidth(int newWidth)
        {
            if (_screenWidth == newWidth)
            {
                return;
            }
            _screenWidth = newWidth;
            CalculateNewDisplayDimensions();
        }

        protected virtual void SetScreenHeight(int newHeight)
        {
            if (_screenHeight == newHeight)
            {
                return;
            }
            _screenHeight = newHeight;
            CalculateNewDisplayDimensions();
        }
        protected virtual void CalculateNewDisplayDimensions()
        {
            if (_characterSize.Height < 0 || _characterSize.Width < 0)
                return;

            Lines = (int)Math.Floor((float)_screenHeight / (float)_characterSize.Height);
            Cols = (int)Math.Floor((float)_screenWidth / (float)_characterSize.Width);
        }

        //protected virtual void SetLineHeight(int newLineHeight)
        //{
        //    if (_lineHeight == newLineHeight)
        //    {
        //        return;
        //    }
        //    _lineHeight = newLineHeight;
        //    CalculateNewLineCount();
        //}

        //protected virtual void CalculateNewLineCount()
        //{
        //    if(_lineHeight <= 0)
        //        return;
        //    Lines = (int)Math.Floor((float)_screenHeight / (float)_lineHeight);
        //}

        protected virtual void OnUpdateDisplay()
        {
            if (_bufferHandler == null)
                return;
            CreateDisplayBuffer(_bufferHandler.GetOutputLines());
            if (_displayBuffer == null)
                return;
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
