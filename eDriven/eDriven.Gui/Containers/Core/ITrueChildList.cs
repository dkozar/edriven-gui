using System.Collections.Generic;
using eDriven.Gui.Components;

namespace eDriven.Gui.Containers
{
    ///<summary>
    /// The interface used for retrieving the "true" children of a DisplayObjectContainer
    /// This is because the Container uses the content pane for handling children (if scrollable)
    /// Children of the Container return the children of content pane
    /// QChildren returns the content pane itself
    ///</summary>
    public interface ITrueChildList
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// The child components of the container
        /// </summary>
        List<DisplayListMember> QChildren { get; }

        /// <summary>
        /// Number of children
        /// </summary>
        /// <returns></returns>
        int QNumberOfChildren { get; }

        /// <summary>
        /// Checks if this is a Owner of a component
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        bool QHasChild(DisplayListMember child);

        /// <summary>
        /// Returns true if child is the descendant of the component or the component itself
        /// Non-exclusive variant
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        bool QContains(DisplayListMember child);

        /// <summary>
        /// Returns true if child is the descendant of the component or the component itself
        /// </summary>
        /// <param name="child"></param>
        /// <param name="includeThisCheck">Should the container (me) be included in the search</param>
        /// <returns></returns>
        bool QContains(DisplayListMember child, bool includeThisCheck);

        /// <summary>
        /// Adds a child to the container
        /// </summary>
        /// <param name="child">A child</param>
        DisplayListMember QAddChild(DisplayListMember child);

        /// <summary>
        /// Adds a child to the container to the specified index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">Child index</param>
        DisplayListMember QAddChildAt(DisplayListMember child, int index);

        /// <summary>
        /// Removes a child from the container
        /// </summary>
        /// <param name="child">A child</param>
        DisplayListMember QRemoveChild(DisplayListMember child);

        /// <summary>
        /// Adds a child from the container at specified index
        /// </summary>
        DisplayListMember QRemoveChildAt(int index);

        /// <summary>
        /// Removes all children from the container
        /// </summary>
        void QRemoveAllChildren();

        ///<summary>
        /// Swaps two children
        ///</summary>
        ///<param name="firstChild">First child</param>
        ///<param name="secondChild">Second child</param>
        void QSwapChildren(DisplayListMember firstChild, DisplayListMember secondChild);

        /// <summary>
        /// Gets child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        DisplayListMember QGetChildAt(int index);

        /// <summary>
        /// Gets child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        int QGetChildIndex(DisplayListMember child);

        /// <summary>
        /// Sets child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">New index</param>
        /// <returns>The position</returns>
        void QSetChildIndex(DisplayListMember child, int index);

        // ReSharper restore InconsistentNaming
    }
}