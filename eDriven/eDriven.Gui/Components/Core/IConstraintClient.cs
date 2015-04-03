namespace eDriven.Gui.Components
{
    /// <summary>
    /// The ability to participate in the constraint layout
    /// </summary>
    public interface IConstraintClient
    {
        /// <summary>
        /// Returns the specified constraint value
        /// </summary>
        /// <param name="constraintName"></param>
        /// <returns></returns>
        object GetConstraintValue(string constraintName);

        /// <summary>
        /// Sets the specified constraint value
        /// </summary>
        /// <param name="constraintName"></param>
        /// <param name="value"></param>
        void SetConstraintValue(string constraintName, object value);
    }
}