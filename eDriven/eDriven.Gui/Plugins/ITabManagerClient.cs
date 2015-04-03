using System.Collections.Generic;
using eDriven.Gui.Components;

namespace eDriven.Gui.Plugins
{
    /// <summary>
    /// The ability to be controlled by tab and arrow keys
    /// </summary>
    public interface ITabManagerClient
    {
        /// <summary>
        /// Exposes the children what can be focused/unfocused
        /// </summary>
        /// <returns></returns>
        List<DisplayListMember> GetTabChildren();

        /// <summary>
        /// Should tabbing be wrapped around
        /// </summary>
        bool CircularTabs { get; set; }

        /// <summary>
        /// Should arrowing be wrapped around
        /// </summary>
        bool CircularArrows { get; set; }
    }
}