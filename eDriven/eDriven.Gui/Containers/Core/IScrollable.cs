using eDriven.Core.Geom;

namespace eDriven.Gui.Containers
{
    /// <summary>
    /// Provides container scrolling
    /// </summary>
    public interface IScrollable
    {
        /// <summary>
        ///  Content scrolling
        /// </summary>
        //bool ScrollContent { get; set; }
        bool ClipContent { get; set; }

        /// <summary>
        /// Scroll position of container's content
        /// </summary>
        //Point ScrollPosition { get; }

        /// <summary>
        /// Horizontal scroll position
        /// </summary>
        float HorizontalScrollPosition { get; set; }

        /// <summary>
        /// Vertical scroll position
        /// </summary>
        float VerticalScrollPosition { get; set;  }

        /// <summary>
        /// The amount by which the container will scroll vertically for each mouse wheel tick
        /// </summary>
        float MouseWheelStep { get; set; }
        
        //bool CanScroll(float scrollBy);

        /// <summary>
        /// Scrolls the scrollable container<br/>
        /// Returns the residuum (unscrolled pixels)<br/>
        /// Mouse even dispatcher could decide to use the residuum to scroll the parent scrollable container
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        Point ScrollBy(Point amount);
    }
}