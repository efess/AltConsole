using AltConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AltConsole
{
    public class WpfControlInput : IInputProvider
    {
        public event EventHandler<InputKeyEventArgs> Input;
        //public event EventHandler<InputEventArgs> Input;

        public WpfControlInput(UIElement wpfControl)
        {
            wpfControl.KeyDown += wpfControl_KeyDown;
            wpfControl.TextInput += wpfControl_TextInput;
        }

        private char? ToUnicode(Key keyDown, bool shift, bool alt)
        {
            char?  newChar = null;

            int virtualKey = KeyInterop.VirtualKeyFromKey(keyDown);

            uint scanCode = Win32Input.MapVirtualKey((uint)virtualKey, Win32Input.MapType.MAPVK_VK_TO_VSC);
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            //Win32Input.GetKeyboardState(keyboardState);
            if (shift)
                keyboardState[16] = 0xff;//(int)Keys.ShiftKey
            if (alt)
            {
                keyboardState[17] = 0xff;//(int)Keys.ControlKey
                keyboardState[18] = 0xff;//(int)Keys.Menu]
            }
            var result = Win32Input.ToUnicode((uint)virtualKey, scanCode, keyboardState, buf, 256, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                        newChar = buf[0];
                    break;
                default:
                        newChar = buf[0];
                        break;
            }
            return newChar;
        }
        void wpfControl_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //var lol = e.Text.ToCharArray();
            //if(lol.Length > 1)
            //{
            //    System.Diagnostics.Debug.Fail("Okay, TextInput contains more than one char...");
            //} 
            //else if(lol.Length == 0)
            //{
            //    System.Diagnostics.Debug.Fail("TextInput doesn't contain anything!");
            //}
            //var chr = lol[0];

            //OnCharacterInput(chr);
        }

        void wpfControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyEvent = new KeyDown
            {
                Character = ToUnicode(e.Key, Keyboard.Modifiers.HasFlag(ModifierKeys.Shift), Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) ?? '\0',
                Key = e.Key,
                IsAlt = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt),
                IsShift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift),
                IsCtrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control),
                IsWindows = Keyboard.Modifiers.HasFlag(ModifierKeys.Windows)
            };
            var evnt = Input;
            if (evnt != null)
            {
                evnt(this, new InputKeyEventArgs(keyEvent));
            }
            
        }

        protected virtual void OnCharacterInput(char inputCharacter)
        {
            return;
        }
    }
}
