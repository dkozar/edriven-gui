using eDriven.Core.Events;
using eDriven.Gui.Components;

namespace eDriven.Gui.Events
{
    public class ScrollEvent : Event
    {
// ReSharper disable InconsistentNaming
        public const string SCROLL = "scroll";
// ReSharper restore InconsistentNaming

        public float Delta;

        public string Detail;

        public Direction Direction;

        public float Position;

        public ScrollEvent(string type) : base(type)
        {
        }

        public ScrollEvent(string type, object target) : base(type, target)
        {
        }

        public ScrollEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public ScrollEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public ScrollEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}, Detail: {1}, Position: {2}, Delta: {3}, Direction: {4}", base.ToString(), Detail, Position, Delta, Direction);
        }
    }
}