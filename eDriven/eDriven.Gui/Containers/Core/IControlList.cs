using System.Collections.Generic;
using eDriven.Gui.Components;

namespace eDriven.Gui.Containers
{
    ///<summary>
    /// The ability to have children
    ///</summary>
    public interface IContentChildList
    {
        /// <summary>
        /// Content group children
        /// </summary>
        List<DisplayListMember> ContentChildren { get; }

        /// <summary>
        /// Number of content group children
        /// </summary>
        int NumberOfContentChildren { get; }

        ///// <summary>
        ///// Content group layout descriptor
        ///// </summary>
        //LayoutDescriptor ContentLayoutDescriptor { get; set; }
        
        ///// <summary>
        ///// Content group layout
        ///// </summary>
        //Layout.ILayout ContentLayout { get; set; }

        ///// <summary>
        ///// Content clip content
        ///// </summary>
        //bool ContentClipContent { get; set; }
        
        ///// <summary>
        ///// Content scroll content
        ///// </summary>
        //bool ContentScrollContent { get; set; }

        /// <summary>
        /// Checks if the content group has a specified child
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        bool HasContentChild(DisplayListMember child);

        /// <summary>
        /// Checks if the content group has a specified descendant
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        bool ContentContains(DisplayListMember child);

        /// <summary>
        /// Checks if the content group has a specified descendant
        /// </summary>
        /// <param name="child"></param>
        /// <param name="includeThisCheck"></param>
        /// <returns></returns>
        bool ContentContains(DisplayListMember child, bool includeThisCheck);

        /// <summary>
        /// Adds a content group child
        /// </summary>
        /// <param name="child">A child</param>
        DisplayListMember AddContentChild(DisplayListMember child);

        /// <summary>
        /// Adds a content group child at specified index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">Child index</param>
        DisplayListMember AddContentChildAt(DisplayListMember child, int index);

        /// <summary>
        /// Removes a content group child
        /// </summary>
        /// <param name="child">A child</param>
        DisplayListMember RemoveContentChild(DisplayListMember child);

        /// <summary>
        /// Removes a content group child at specified index
        /// </summary>
        DisplayListMember RemoveContentChildAt(int index);

        /// <summary>
        /// Removes all content group children
        /// </summary>
        void RemoveAllContentChildren();

        ///<summary>
        /// Swaps content group children
        ///</summary>
        ///<param name="firstChild">First child</param>
        ///<param name="secondChild">Second child</param>
        void SwapContentChildren(DisplayListMember firstChild, DisplayListMember secondChild);

        /// <summary>
        /// Gets content group child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        DisplayListMember GetContentChildAt(int index);

        /// <summary>
        /// Gets content group child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        int GetContentChildIndex(DisplayListMember child);
            
    }
}