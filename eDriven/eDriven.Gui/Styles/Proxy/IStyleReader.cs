namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The ability of serving as style proxy
    /// </summary>
    public interface IStyleReader //<T>
    {
        /// <summary>
        /// Gets the component ID
        /// </summary>
        /// <returns></returns>
        string GetId(object current);

        /// <summary>
        /// Gets the component classname
        /// </summary>
        /// <returns></returns>
        string GetClassname(object current);

        /// <summary>
        /// Gets the parent based on supplied component
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        object GetParent(/*T*/object current);
    }
}