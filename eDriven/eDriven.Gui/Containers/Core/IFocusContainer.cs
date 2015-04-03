namespace eDriven.Gui.Containers
{
    /// <summary>
    /// The ability to receive focus
    /// </summary>
    public interface IFocusContainer
    {
        /// <summary>
        /// Is direct child focused
        /// </summary>
        bool HasFocusedChild { get; }

        /// <summary>
        /// Is descendant focused (which could be direct child also)
        /// </summary>
        bool HasFocusedDescendant { get; }
    }
}