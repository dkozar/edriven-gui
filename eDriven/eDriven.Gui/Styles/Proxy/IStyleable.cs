namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The component having the ability to consume styles
    /// </summary>
    public interface IStyleable
    {
        /// <summary>
        /// Sets the style on the component
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="value"></param>
        void SetStyle(string styleName, object value);

        /// <summary>
        /// Returns the style specified with style name
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        object GetStyle(string styleName);

        /// <summary>
        /// Notifies the style change
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="value"></param>
        void StyleChanged(string styleName, object value);
    }
}