using eDriven.Core.Events;

namespace eDriven.Extensions.Login
{
    public class LoginEvent : Event
    {
// ReSharper disable InconsistentNaming
        public const string LOGIN = "login";
// ReSharper restore InconsistentNaming

        public string Username;
        public string Password;

        public LoginEvent(string type) : base(type)
        {
        }

        public LoginEvent(string type, object target) : base(type, target)
        {
        }

        public LoginEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public LoginEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public LoginEvent(string type, object target, bool bubbles, bool cancelable)
            : base(type, target, bubbles, cancelable)
        {
        }
    }
}
