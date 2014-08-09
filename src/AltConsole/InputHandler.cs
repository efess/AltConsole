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
            _currentInput = string.Empty;
            provider.Input += (s,e) => { OnInputKey(e.InputKey);};
            
            _inputHistory = new InputHistory();
        }

        private void OnInputKey(KeyDown inputKey)
        {
            char? chr;
            if (inputKey.Key == Key.Return || inputKey.Key == Key.Enter)
            {
                _currentHash = Guid.Empty;
                _currentInput = _currentInput + Environment.NewLine;
                OnCommand(_currentInput);
                _inputHistory.AddPrevious(_currentInput);
                _currentInput = string.Empty;
                _position = 0;
                OnInputChanged(_currentInput, _position);
            }
            else if (inputKey.Key == Key.Back)
            {
                if (_currentInput.Length == 0 || _position <= -_currentInput.Length)
                    return;
                _currentInput = _currentInput.Remove(_currentInput.Length + _position-1, 1);
            }
            else if (inputKey.Key == Key.Home)
            {
                _position = -_currentInput.Length;
            }
            else if (inputKey.Key == Key.End)
            {
                _position = 0;
            }
            else if (inputKey.Key == Key.Delete)
            {
                if (_position < 0)
                {
                    _currentInput = _currentInput.Remove(_currentInput.Length + _position, 1);
                    _position++;
                }
            }
            else if (inputKey.Key == Key.Left)
            {
                if (_position > (1 - (_currentInput.Length + 1)))
                    _position--;
            }
            else if (inputKey.Key == Key.Right)
            {
                if (_position < 0)
                    _position++;
            }
            else if (inputKey.Key == Key.Up)
            {
                var prev = _inputHistory.GetPrevious(_currentHash);
                if (prev != null)
                {
                    _currentHash = prev.Value.Key;
                    _currentInput = prev.Value.Value;
                    _position = _currentInput.Length;
                }
            }
            else if (inputKey.Key == Key.Down)
            {
                var next = _inputHistory.GetNext(_currentHash);
                if (next != null)
                {
                    _currentHash = next.Value.Key;
                    _currentInput = next.Value.Value;
                    _position = 0;
                }
                else
                {

                    _currentHash = Guid.Empty;
                    _currentInput = "";
                    _position = 0;
                }
            }
            else if (inputKey.Character != '\0')
            {
                // try get Character
                if (_position == 0)
                {
                    _currentInput += inputKey.Character;
                }
                else
                {
                    _currentInput = _currentInput.Insert(_currentInput.Length + _position, inputKey.Character.ToString());
                    //_position++;
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
    }
}
