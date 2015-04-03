namespace eDriven.Gui.Components
{
    ///<summary>
    /// The ability of a component to be validated
    ///</summary>
    public interface INestLevel
    {
        /// <summary>
        /// The hierarchy depth
        /// </summary>
        int NestLevel { get; set; }
    }
}