using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltConsole
{
    public class InputHistory
    {
        private List<KeyValuePair<Guid, string>> _history; //hash, input value
        public InputHistory()
        {
            _history = new List<KeyValuePair<Guid, string>>();
        }

        public KeyValuePair<Guid, string> AddPriinevoius(string input)
        {
            if(_history.Count > 0 && _history[_history.Count - 1].Value == input)
                return _history[_history.Count - 1];

            var history = new KeyValuePair<Guid, string>(Guid.NewGuid(), input);
            _history.Add(history);
            return history;
        }

        public KeyValuePair<Guid, string>? GetPrevious(Guid hash)
        {
            if (hash == Guid.Empty && _history.Count > 0)
                return _history[_history.Count - 1];
            for (int i = _history.Count - 1; i > 0; i--)
            {
                if (_history[i].Key == hash)
                    return _history[i - 1];
            }
            return null;
        }

        public KeyValuePair<Guid, string>? GetNext(Guid hash)
        {
            if (hash == Guid.Empty)
                return null;// Can't get "next" if we don't have a current.
            for (int i = _history.Count - 2; i >= 0; i--)
            {
                if (_history[i].Key == hash)
                    return _history[i + 1];
            }
            return null;
        }

        public KeyValuePair<Guid, string>? SearchPrevious(string currentInput, Guid hash)
        {
            bool hashFound = hash == Guid.Empty;
            for (int i = _history.Count - 1; i > 0; i--)
            {
                var history = _history[i];

                if (hashFound && history.Value.StartsWith(currentInput))
                    return history;
                hashFound = hashFound || hash == history.Key;
            }
            return null;
        }

    }
}
