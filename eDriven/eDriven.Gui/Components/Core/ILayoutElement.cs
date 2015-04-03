using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    ///<summary>
    /// The ability to be included in layout passes
    ///</summary>
    public interface ILayoutElement : IPercentage, IConstraintClient//, IEventDispatcher
    {
        /// <summary>
        /// Horizontal distance to the left edge of the parent container
        /// </summary>
        object Left { get; set; }

        /// <summary>
        /// Horizontal distance to the right edge of the parent container
        /// </summary>
        object Right { get; set; }

        /// <summary>
        /// Vertical distance to the top edge of the parent container
        /// </summary>
        object Top { get; set; }

        /// <summary>
        /// Vertical distance to the bottom edge of the parent container
        /// </summary>
        object Bottom { get; set; }

        /// <summary>
        /// Horizontal offset of the component center to the container center
        /// </summary>
        object HorizontalCenter { get; set; }

        /// <summary>
        /// Vertical offset of the component center to the container center
        /// </summary>
        object VerticalCenter { get; set; }

        /// <summary>
        /// The flag indicating tha a component is included in measurement and layout
        /// </summary>
        bool IncludeInLayout { get; set; }

        ///<summary>
        ///</summary>
        ///<param name="x"></param>
        ///<param name="y"></param>
        void SetLayoutBoundsPosition(float x, float y);

        ///<summary>
        ///</summary>
        ///<param name="width"></param>
        ///<param name="height"></param>
        void SetLayoutBoundsSize(float? width, float? height);
    }
}