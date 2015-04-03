using eDriven.Core.Events;

namespace eDriven.Gui.Events
{
    /// <summary>
    /// Window close event
    /// </summary>
    public class CloseEvent : Event
    {
        public const string CLOSE = "close";

        public CloseEvent(string type)
            : base(type)
        {
            Bubbles = true;
        }

        public CloseEvent(string type, object target)
            : base(type, target)
        {
            Bubbles = true;
        }
    }
}