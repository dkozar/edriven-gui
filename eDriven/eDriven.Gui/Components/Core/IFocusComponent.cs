namespace eDriven.Gui.Components
{
    /// <summary>
    /// Defines methods which the component got to have to receive focus
    /// </summary>
    public interface IFocusComponent
    {
        /// <summary>
        /// Sets component to focus
        /// </summary>
        void SetFocus();

        /// <summary>
        /// Does a component have focus (including its children)
        /// </summary>
        bool HasFocus { get; }

        /// <summary>
        /// Does a component have an explicit focus
        /// </summary>
        bool HasExplicitFocus { get; }
    }
}