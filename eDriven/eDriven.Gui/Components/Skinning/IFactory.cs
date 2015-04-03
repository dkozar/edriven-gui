namespace eDriven.Gui.Components
{
    /// <summary>
    /// Creates instances of the object
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Creates an instance of th eobject
        /// </summary>
        /// <returns></returns>
        object NewInstance();
    }
}