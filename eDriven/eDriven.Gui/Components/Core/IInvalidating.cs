namespace eDriven.Gui.Components
{
    ///<summary>
    /// The ability of a component to be invalidated
    ///</summary>
    public interface IInvalidating
    {
        /// <summary>
        /// A lifecycle method
        /// Invalidates properties
        /// </summary>
        void InvalidateProperties();

        /// <summary>
        /// A lifecycle method
        /// Invalidates size
        /// </summary>
        void InvalidateSize();

        /// <summary>
        /// A lifecycle method
        /// Invalidates layout
        /// </summary>
        void InvalidateDisplayList();

        /// <summary>
        /// A lifecycle method
        /// Validates all
        /// </summary>
        void ValidateNow();
    }
}