namespace eDriven.Gui.Components
{
    /// <summary>
    /// Having the ability to contain the arbitrary data
    /// </summary>
    public interface IDataHost
    {
        /// <summary>
        /// The arbitrary data attached to this piece of GUI
        /// </summary>
        object Data { get; set; }
    }
}