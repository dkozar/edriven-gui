namespace eDriven.Gui.Components
{
    ///<summary>
    ///</summary>
    public interface IFocusManagerComponent
    {
        bool FocusEnabled { get; set; }

        bool HasFocusableChildren { get; set; }

        bool MouseFocusEnabled { get; }

        int TabIndex { get; set; }

        void SetFocus();

        void DrawFocus(bool isFocused);
    }
}