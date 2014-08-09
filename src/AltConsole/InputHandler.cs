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
            else if (inputKey.Character != '\0')
            {
                // try get Character
                _currentInput += inputKey.Character;
                _position++;
                
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
            char? newChar = alphaKeyToChar(someKey, isShift);
            if (newChar == null)
                newChar = specialKeyToChar(someKey, isShift);
            return newChar;
        }

        private char? specialKeyToChar(Key someKey, bool isShift)
        {
            switch (someKey)
            { 
                case Key.Space:
                    return ' ';
                case Key.D0:
                    return isShift ? ')' : '0';
                case Key.D1:
                    return isShift ? '!' : '1';
                case Key.D2:
                    return isShift ? '@' : '2';
                case Key.D3:
                    return isShift ? '#' : '3';
                case Key.D4:
                    return isShift ? '$' : '4';
                case Key.D5:
                    return isShift ? '%' : '5';
                case Key.D6:
                    return isShift ? '^' : '6';
                case Key.D7:
                    return isShift ? '&' : '7';
                case Key.D8:
                    return isShift ? '*' : '8';
                case Key.D9:
                    return isShift ? '(' : '9';
                case Key.OemMinus:
                    return isShift ? '_' : '-';
                case Key.OemPlus:
                    return isShift ? '+' : '=';
                case Key.OemOpenBrackets:
                    return isShift ? '{' : '[';
                case Key.OemCloseBrackets:
                    return isShift ? '}' : ']';
                case Key.OemSemicolon:
                    return isShift ? ':' : ';';
                case Key.OemQuotes:
                    return isShift ? '\'' : '\"';
                case Key.OemBackslash:
                    return isShift ? '|' : '\\';
                case Key.OemComma:
                    return isShift ? '<' : ',';
                case Key.OemPeriod:
                    return isShift ? '>' : '.';
                case Key.OemQuestion:
                    return isShift ? '?' : '/';
                case Key.OemTilde:
                    return isShift ? '~' : '`';
                default:
                    return null;
            }
        }

        private char? alphaKeyToChar(Key someKey, bool isShift)
        {
            char? newChar = null;
            switch (someKey)
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
            if (newChar != null && isShift)
            {
                newChar = (char)((int)newChar - 32);
            }
            return newChar;
        }
    }
}
