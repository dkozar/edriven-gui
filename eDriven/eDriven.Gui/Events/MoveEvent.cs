using eDriven.Core.Events;
using eDriven.Core.Geom;

namespace eDriven.Gui.Events
{
    public class MoveEvent : Event
    {
// ReSharper disable InconsistentNaming
        public const string MOVE = "move";
// ReSharper restore InconsistentNaming

        public Point Position;
        public float OldX;
        public float OldY;

        public MoveEvent(string type) : base(type)
        {
        }

        public MoveEvent(string type, object target) : base(type, target)
        {
        }

        public MoveEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public MoveEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }
    }
}
