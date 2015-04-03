using eDriven.Gui.Styles;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Check box
    /// </summary>
    [Style(Name = "skinClass", Default = typeof(CheckBoxSkin))]

    public class CheckBox : Button
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CheckBox()
        {
            ToggleMode = true;
        }
    }
}
