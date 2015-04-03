using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    public class FocusEvent : Event
    {
        public const string FOCUS_IN = "focusIn";
        public const string FOCUS_OUT = "focusOut";

        public DisplayListMember RelatedObject { get; set; }

        public FocusEvent(string type) : base(type)
        {
        }

        public FocusEvent(string type, object target) : base(type, target)
        {
        }

        public FocusEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public FocusEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public FocusEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
        {
        }
    }
}