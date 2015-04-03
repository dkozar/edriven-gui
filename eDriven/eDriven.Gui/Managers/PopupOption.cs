namespace eDriven.Gui.Managers
{
    public class PopupOption
    {
        public PopupOptionType Type;
        public object Value;

        public PopupOption(PopupOptionType type, object value)
        {
            Type = type;
            Value = value;
        }
    }

    public enum PopupOptionType
    {
        Parent, 
        Modal, 
        Centered, 
        KeepCenter, 
        RemoveOnMouseDownOutside, 
        RemoveOnMouseWheelOutside,
        RemoveOnScreenResize,
        AutoFocus,
        FocusPreviousOnHide,
        Stage
    }
}
