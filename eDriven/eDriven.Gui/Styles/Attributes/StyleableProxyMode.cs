namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The mode of calling methods on target object
    /// </summary>
    public enum StyleableProxyMode
    {
        /// <summary>
        /// Using Unity's SendMessage
        /// </summary>
        SendMessage, 
        
        /// <summary>
        /// Using reflection
        /// </summary>
        Reflection
    }
}