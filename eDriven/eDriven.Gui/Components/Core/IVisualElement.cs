using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    ///<summary>
    /// The ability to be displayed in the hierarchy
    ///</summary>
    public interface IVisualElement : IEventDispatcher
    {
        /// <summary>
        /// A parent
        /// </summary>
        DisplayObjectContainer Parent { get; set; }

        /// <summary>
        /// The owner
        /// </summary>
        DisplayObject Owner { get; set; }

        /// <summary>
        /// Depth
        /// </summary>
        int Depth { get; set;  }

        /// <summary>
        /// Visible
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Alpha
        /// </summary>
        float Alpha { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        float Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        float Height { get; set; }

        /// <summary>
        /// X
        /// </summary>
        float X { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        float Y { get; set; }

        /// <summary>
        /// True for including in layout
        /// </summary>
        bool IncludeInLayout { get; set; }
    }
}