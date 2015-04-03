using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    internal class DropDownEvent : Event
    {
// ReSharper disable InconsistentNaming
        public const string OPEN = "open";
        public const string CLOSE = "close";
// ReSharper restore InconsistentNaming

        public DropDownEvent(string type) : base(type)
        {
        }

        public DropDownEvent(string type, object target) : base(type, target)
        {
        }

        public DropDownEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public DropDownEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public DropDownEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
        {
        }
    }
}