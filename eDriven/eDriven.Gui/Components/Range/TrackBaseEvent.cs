using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    public class TrackBaseEvent : Event
    {
        public const string THUMB_DRAG = "thumbDrag";
        public const string THUMB_PRESS = "thumbPress";
        public const string THUMB_RELEASE = "thumbRelease";

        public TrackBaseEvent(string type) : base(type)
        {
        }

        public TrackBaseEvent(string type, object target) : base(type, target)
        {
        }

        public TrackBaseEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public TrackBaseEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public TrackBaseEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
        {
        }
    }
}
