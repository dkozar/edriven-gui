namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The ability to produce the set of style values
    /// </summary>
    public interface IStyleValuesFactory
    {
        /// <summary>
        /// Reads the serialized declaration and extracts key/value pairs
        /// </summary>
        /// <returns></returns>
        StyleTable Produce();
    }
}