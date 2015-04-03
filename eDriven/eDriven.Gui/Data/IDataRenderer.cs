namespace eDriven.Gui.Data
{
    ///<summary>
    /// The ability to attach a piece of arbitrary data
    ///</summary>
    public interface IDataRenderer
    {
        /// <summary>
        /// The attached data
        /// </summary>
        object Data { get; set; }
    }
}