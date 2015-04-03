using eDriven.Gui.Components;

namespace eDriven.Gui.Form
{
    public class TextAreaFormAdapter : FormAdapterBase<TextArea>
    {
        #region Implementation of IFormAdapter

        public override object GetValue(Component component)
        {
            return Convert(component).Text;
        }

        /// <summary>
        /// Sets the component value
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(Component component, object value)
        {
            Convert(component).Text = (string)value;
        }

        /// <summary>
        /// Resets the component to its default value
        /// </summary>
        public override void Reset(Component component)
        {
            Convert(component).Text = string.Empty;
        }

        #endregion
    }
}