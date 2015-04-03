namespace eDriven.Gui.Components
{
    /// <summary>
    /// The ability to be sized by the parent container by percentages of container's dimensions
    /// </summary>
    public interface IPercentage
    {
        ///<summary>
        /// The percent width of the component (in pixels)<br/>
        /// When set, it resets ExplicitWidth property to null
        ///</summary>
        float? PercentWidth { get; set; }

        ///<summary>
        /// The percent height of the component (in pixels)<br/>
        /// When set, it resets ExplicitHeight property to null
        ///</summary>
        float? PercentHeight { get; set; }
    }
}