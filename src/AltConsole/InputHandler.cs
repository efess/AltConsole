using AltConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AltConsole
{
    public class InputHandler
    {
        public EventHandler<InputEventArgs> InputChanged;
        public EventHandler<InputLineEventArgs> Command;
        private InputHistory _inputHistory;

        private int _position;
        private string _currentInput;
        private Guid _currentHash;

        public InputHandler(IInputProvider provider)
        {
            if(provider == null)
                throw new ArgumentNullException("provider");
            provider.Input += (s,e) => { OnInputKey(e.InputKey);};
            
            _inputHistory = new InputHistory();
        }

        private void OnInputKey(KeyDown inputKey)
        {
            char? chr;
            if (inputKey.Key == Key.Return || inputKey.Key == Key.Enter)
            {
                _currentInput = _currentInput + Environment.NewLine;
                OnCommand(_currentInput);
                
                _currentInput = string.Empty;
                _position = 0;
                OnInputChanged(_currentInput, _position);
            }
            else if (inputKey.Key == Key.Back)
            {
                if (_currentInput.Length == 0 || _position == 0)
                    return;
                _position--;
                _currentInput = _currentInput.Remove(_position, 1);
            }
            else if (inputKey.Key == Key.Left)
            {
                if (_position > 0)
                    _position--;
            }
            else if (inputKey.Key == Key.Right)
            {
                if (_position < _currentInput.Length-1)
                    _position++;
            }
            else if ((chr = keyToChar(inputKey.Key, inputKey.IsShift)) != null)
            {
                if (chr != null)
                {
                    // try get Character
                    _currentInput += chr;
                    _position++;
                }
            }
            else
            {
                return;
            }
            OnInputChanged(_currentInput, _position);
        }

        protected virtual void OnCommand(string command)
        {
            var evnt = Command;
            if (evnt != null)
            {
                evnt(this, new InputLineEventArgs(command));
            }
        }

        protected virtual void OnInputChanged(string input, int position)
        {
            var evnt = InputChanged;
            if(evnt != null)
            {
                evnt(this, new InputEventArgs(input, position));
            }
        }

        private char? keyToChar(Key someKey, bool isShift)
        {
            char? newChar = null;
            switch(someKey)
            {
                case Key.A:
                    newChar = 'a';
                    break;
                case Key.B:
                    newChar = 'b';
                    break;
                case Key.C:
                    newChar = 'c';
                    break;
                case Key.D:
                    newChar = 'd';
                    break;
                case Key.E:
                    newChar = 'e';
                    break;
                case Key.F:
                    newChar = 'f';
                    break;
                case Key.G:
                    newChar = 'g';
                    break;
                case Key.H:
                    newChar = 'h';
                    break;
                case Key.I:
                    newChar = 'i';
                    break;
                case Key.J:
                    newChar = 'j';
                    break;
                case Key.K:
                    newChar = 'k';
                    break;
                case Key.L:
                    newChar = 'l';
                    break;
                case Key.M:
                    newChar = 'm';
                    break;
                case Key.N:
                    newChar = 'n';
                    break;
                case Key.O:
                    newChar = 'o';
                    break;
                case Key.P:
                    newChar = 'p';
                    break;
                case Key.Q:
                    newChar = 'q';
                    break;
                case Key.R:
                    newChar = 'r';
                    break;
                case Key.S:
                    newChar = 's';
                    break;
                case Key.T:
                    newChar = 't';
                    break;
                case Key.U:
                    newChar = 'u';
                    break;
                case Key.V:
                    newChar = 'v';
                    break;
                case Key.W:
                    newChar = 'w';
                    break;
                case Key.X:
                    newChar = 'x';
                    break;
                case Key.Y:
                    newChar = 'y';
                    break;
                case Key.Z:
                    newChar = 'z';
                    break;
            }
            if(newChar != null && isShift)
            {
                newChar = (char)((int)newChar + 26);
            }
            return newChar;
        }
    }
}
