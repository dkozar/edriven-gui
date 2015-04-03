namespace eDriven.Gui.Components
{
    internal class StateChangeEvent : Core.Events.Event
    {
        public const string CURRENT_STATE_CHANGING = "currentStateChanging";
        public const string CURRENT_STATE_CHANGE = "currentStateChange";

        public string OldState;
        public string NewState;

        public StateChangeEvent(string type) : base(type)
        {
        }
    }
}