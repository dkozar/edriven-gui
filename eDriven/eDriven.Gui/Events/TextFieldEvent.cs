using eDriven.Core.Events;

namespace eDriven.Gui.Events
{
    public class TextFieldEvent : KeyboardEvent
    {
        #region CONSTANTS

        // ReSharper disable InconsistentNaming
        public const string TEXT_CHANGING = "textChanging";
        public const string TEXT_CHANGE = "textChange";
        // ReSharper restore InconsistentNaming

        #endregion

        /// <summary>
        /// The last typed character
        /// </summary>
        public char Character;

        /// <summary>
        /// The text befory typing
        /// </summary>
        public string OldText;

        /// <summary>
        /// The text after typing
        /// </summary>
        public string NewText;

        public TextFieldEvent(string type, object target) : base(type, target)
        {
        }

        public TextFieldEvent(KeyboardEvent keyboardEvent) : base(keyboardEvent.Type, keyboardEvent.Target)
        {
            KeyCode = keyboardEvent.KeyCode;
            Control = keyboardEvent.Control;
            Shift = keyboardEvent.Shift;
            Alt = keyboardEvent.Alt;
            Character = keyboardEvent.CurrentEvent.character;
        }

        public TextFieldEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public TextFieldEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public TextFieldEvent(string type) : base(type)
        {
        }
    }
}