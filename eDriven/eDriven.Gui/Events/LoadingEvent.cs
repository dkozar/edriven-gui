using eDriven.Core.Events;

namespace eDriven.Gui.Events
{
    public class LoadingEvent : Event
    {
        // ReSharper disable InconsistentNaming
        public const string START = "loadingStart";
        public const string PROGRESS = "loadingStart";
        public const string END = "loadingEnd";
        public const string ERROR = "loadingError";
        // ReSharper restore InconsistentNaming

        public string Message;

        public LoadingEvent(string type, string message)
            : base(type)
        {
            Message = message;
        }

        public LoadingEvent(string type) : base(type)
        {
        }

        public LoadingEvent(string type, object target) : base(type, target)
        {
        }

        public LoadingEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public LoadingEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }
    }
}
