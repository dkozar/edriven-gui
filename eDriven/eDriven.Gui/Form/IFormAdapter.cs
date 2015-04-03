using eDriven.Gui.Components;

namespace eDriven.Gui.Form
{
    /// <summary>
    /// Form adapter knows how to get, set and reset the value of the control
    /// </summary>
    public interface IFormAdapter
    {
        /// <summary>
        /// Gets the component value
        /// </summary>
        /// <returns></returns>
        object GetValue(Component component);

        /// <summary>
        /// Sets the component value
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        void SetValue(Component component, object value);

        /// <summary>
        /// Resets the component to its default value
        /// </summary>
        void Reset(Component component);
    }
}