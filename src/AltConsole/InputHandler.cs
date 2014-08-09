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
    }
}
