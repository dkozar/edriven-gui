using eDriven.Core.Events;

namespace eDriven.Extensions.ExampleControl2
{
    public class ExampleEvent : Event
    {
// ReSharper disable InconsistentNaming
        public const string SEND_MESSAGE = "sendMessage";
// ReSharper restore InconsistentNaming

        public string To;
        public string Cc;
        public string Bcc;
        public string Message;

        public ExampleEvent(string type) : base(type)
        {
        }

        public ExampleEvent(string type, object target) : base(type, target)
        {
        }

        public ExampleEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public ExampleEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public ExampleEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
        {
        }
    }
}
