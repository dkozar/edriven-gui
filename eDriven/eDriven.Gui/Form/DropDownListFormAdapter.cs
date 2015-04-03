using System;
using eDriven.Gui.Components;

namespace eDriven.Gui.Form
{
    public class DropDownListFormAdapter : FormAdapterBase<DropDownList>
    {
        #region Implementation of IFormAdapter

        /// <summary>
        /// Gets the component value
        /// </summary>
        /// <returns></returns>
        public override object GetValue(Component component)
        {
            return Convert(component).SelectedItem;
        }

        /// <summary>
        /// Sets the component value
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(Component component, object value)
        {
            var combo = Convert(component);
            combo.RequireSelection = false;
            combo.SelectedItem = value;
        }

        /// <summary>
        /// Resets the component to its default value
        /// </summary>
        public override void Reset(Component component)
        {
            var combo = Convert(component);
            combo.RequireSelection = false;
            combo.SelectedIndex = -1;
        }

        #endregion
    }
}