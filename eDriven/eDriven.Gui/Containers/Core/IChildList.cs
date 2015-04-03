using System.Collections.Generic;
using eDriven.Gui.Components;

namespace eDriven.Gui.Containers
{
    ///<summary>
    /// The ability to have children
    ///</summary>
    public interface IChildList
    {
        /// <summary>
        /// The child components of the container
        /// </summary>
        List<DisplayListMember> Children { get; }

        /// <summary>
        /// Number of children
        /// </summary>
        int NumberOfChildren { get; }

        /// <summary>
        /// Checks if this is a Owner of a component
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        bool HasChild(DisplayListMember child);

        /// <summary>
        /// Returns true if child is the descendant of the component or the component itself
        /// Non-exclusive variant
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        bool Contains(DisplayListMember child);

        /// <summary>
        /// Returns true if child is the descendant of the component or the component itself
        /// </summary>
        /// <param name="child"></param>
        /// <param name="includeThisCheck">Should the container (me) be included in the search</param>
        /// <returns></returns>
        bool Contains(DisplayListMember child, bool includeThisCheck);

        /// <summary>
        /// Adds a child to the container
        /// </summary>
        /// <param name="child">A child</param>
        DisplayListMember AddChild(DisplayListMember child);

        /// <summary>
        /// Adds a child to the container to the specified index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">Child index</param>
        DisplayListMember AddChildAt(DisplayListMember child, int index);

        /// <summary>
        /// Removes a child from the container
        /// </summary>
        /// <param name="child">A child</param>
        DisplayListMember RemoveChild(DisplayListMember child);

        /// <summary>
        /// Adds a child from the container at specified index
        /// </summary>
        DisplayListMember RemoveChildAt(int index);

        /// <summary>
        /// Removes all children from the container
        /// </summary>
        void RemoveAllChildren();

        ///<summary>
        /// Swaps two children
        ///</summary>
        ///<param name="firstChild">First child</param>
        ///<param name="secondChild">Second child</param>
        void SwapChildren(DisplayListMember firstChild, DisplayListMember secondChild);

        /// <summary>
        /// Gets child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        DisplayListMember GetChildAt(int index);

        /// <summary>
        /// Gets child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        int GetChildIndex(DisplayListMember child);

        ///<summary>
        /// Sets child index
        ///</summary>
        ///<param name="child"></param>
        ///<param name="index"></param>
        void SetChildIndex(DisplayListMember child, int index);
    }
}