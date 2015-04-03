using eDriven.Core.Events;

namespace eDriven.Gui.Events
{
    /// <summary>
    /// The event that wrapps up UnityEngine.Event.current
    /// Has a reference to UnityEngine.Event.current as 
    /// It bubbles by default and copies Shift, Control and Alt from UnityEngine.Event.current
    /// </summary>
    public class GuiEvent : InputEvent
    {
        // ReSharper disable InconsistentNaming
        public const string FOCUS_IN = "focusIn";
        public const string FOCUS_OUT = "focusOut";
        // ReSharper restore InconsistentNaming

        public GuiEvent(string type)
            : base(type)
        {
            Bubbles = true; // Note: important!
        }

        public GuiEvent(string type, object target)
            : base(type, target)
        {
            Bubbles = true; // Note: important!
        }

        public GuiEvent(string type, bool bubbles) : base(type, bubbles)
        {
            Bubbles = true; // Note: important!
        }

        public GuiEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
            Bubbles = true; // Note: important!
        }

        override public void Cancel()
        {
            base.Cancel();
            if (null != CurrentEvent)
                CurrentEvent.Use();
        }
    }
}