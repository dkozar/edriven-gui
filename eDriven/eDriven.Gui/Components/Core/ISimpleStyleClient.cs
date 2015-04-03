namespace eDriven.Gui.Components
{
    /// <summary>
    /// Used by objects that doesn't need to store style values locally
    /// </summary>
    public interface ISimpleStyleClient
    {
        /// <summary>
        /// Executes when the style changes
        /// </summary>
        /// <param name="styleName"></param>
        void StyleChanged(string styleName);

        /// <summary>
        /// Component identifier
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Style name
        /// </summary>
        object StyleName { get; set; }
    }
}