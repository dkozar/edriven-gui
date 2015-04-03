using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    public class StringEvent : Event
    {
        public string Text;


        public StringEvent(string type) : base(type)
        {
        }

        public StringEvent(string type, object target) : base(type, target)
        {
        }

        public StringEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public StringEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }
    }
}