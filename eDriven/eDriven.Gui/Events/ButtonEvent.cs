namespace eDriven.Gui.Events
{
    /// <summary>
    /// The event that has global and local position
    /// </summary>
    public class ButtonEvent : GuiEvent
    {
        public const string BUTTON_DOWN = "buttonDown";
        public const string BUTTON_UP = "buttonUp";
        public const string PRESS = "press";
        public const string CHANGE = "change";

        public string ButtonId;
        public string ButtonText;
        public bool Selected; // for toggle buttons

        public ButtonEvent(string type)
            : base(type)
        {
            Bubbles = true;
        }

        public ButtonEvent(string type, object target)
            : base(type, target)
        {
            Bubbles = true;
        }
    }
}