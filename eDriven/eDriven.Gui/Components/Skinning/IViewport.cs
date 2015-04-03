namespace eDriven.Gui.Components
{
    ///<summary>
    /// IViewport interface
    ///</summary>
    public interface IViewport : IVisualElement
    {
        /**
         *  The width of the Viewport's contents.
         * 
         *  If <code>clipAndEnabledScrolling</code> is true, the Viewport's 
         *  <code>contentWidth</code> defines the limit for horizontal scrolling 
         *  and the Viewport's actual width defines how much of the content is visible.
         * 
         *  To scroll through the content horizontally, vary the 
         *  <code>horizontalScrollPosition</code> between 0 and
         *  <code>contentWidth - width</code>.  
         */
        ///<summary>
        /// Content width
        ///</summary>
        float ContentWidth { get; }
        
        /**
         *  The height of the Viewport's content.
         * 
         *  If <code>clipAndEnabledScrolling</code> is true, the Viewport's 
         *  <code>contentHeight</code> defines the limit for vertical scrolling 
         *  and the Viewport's actual height defines how much of the content is visible.
         * 
         *  To scroll through the content vertically, vary the 
         *  <code>verticalScrollPosition</code> between 0 and
         *  <code>contentHeight - height</code>.
         */
        ///<summary>
        /// Content height
        ///</summary>
        float ContentHeight { get; }

        /**
         *  The x coordinate of the origin of the Viewport in the component's coordinate system, 
         *  where the default value is (0,0) corresponding to the upper-left corner of the component.
         * 
         *  If <code>clipAndEnableScrolling</code> is <code>true</code>, setting this property 
         *  typically causes the Viewport to be set to:
         *  <pre>
         *  new Rectangle(horizontalScrollPosition, verticalScrollPosition, width, height)
         *  </pre>
         * 
         *  Implementations of this property must be Bindable and
         *  must generate events of type <code>propertyChange</code>.
         */
        ///<summary>
        ///</summary>
        float HorizontalScrollPosition { get; set; }

        /**
         *  The y coordinate of the origin of the Viewport in the component's coordinate system, 
         *  where the default value is (0,0) corresponding to the upper-left corner of the component.
         * 
         *  If <code>clipAndEnableScrolling</code> is <code>true</code>, setting this property 
         *  typically causes the Viewport to be set to:
         *  <pre>
         *  new Rectangle(horizontalScrollPosition, verticalScrollPosition, width, height)
         *  </pre>
         * 
         *  Implementations of this property must be Bindable and
         *  must generate events of type <code>propertyChange</code>.
         */
        float VerticalScrollPosition { get; set; }

        /**
         *  Returns the amount to add to the Viewport's current 
         *  <code>horizontalScrollPosition</code> to scroll by the requested scrolling unit.
         *
         *  Param: navigationUnit The amount to scroll. 
         *  The value must be one of the following NavigationUnit
         *  constants: 
         *  <ul>
         *   <li><code>LEFT</code></li>
         *   <li><code>RIGHT</code></li>
         *   <li><code>PAGE_LEFT</code></li>
         *   <li><code>PAGE_RIGHT</code></li>
         *   <li><code>HOME</code></li>
         *   <li><code>END</code></li>
         *  </ul>
         *  To scroll by a single column, use <code>LEFT</code> or <code>RIGHT</code>.
         *  To scroll to the first or last column, use <code>HOME</code> or <code>END</code>.
         *
         *  Returns: The number of pixels to add to <code>horizontalScrollPosition</code>.
         */
        ///<summary>
        ///</summary>
        ///<param name="navigationUnit"></param>
        ///<returns></returns>
        float GetHorizontalScrollPositionDelta(NavigationUnit navigationUnit);
        
        /**
         *  Returns the amount to add to the Viewport's current 
         *  <code>verticalScrollPosition</code> to scroll by the requested scrolling unit.
         *
         *  Param: navigationUnit The amount to scroll. 
         *  The value of unit must be one of the following NavigationUnit
         *  constants: 
         *  <ul>
         *   <li><code>UP</code></li>
         *   <li><code>DOWN</code></li>
         *   <li><code>PAGE_UP</code></li>
         *   <li><code>PAGE_DOWN</code></li>
         *   <li><code>HOME</code></li>
         *   <li><code>END</code></li>
         *  </ul>
         *  To scroll by a single row use <code>UP</code> or <code>DOWN</code>.
         *  To scroll to the first or last row, use <code>HOME</code> or <code>END</code>.
         *
         *  Returns: The number of pixels to add to <code>verticalScrollPosition</code>.
         */
        ///<summary>
        ///</summary>
        ///<param name="navigationUnit"></param>
        ///<returns></returns>
        float GetVerticalScrollPositionDelta(NavigationUnit navigationUnit);

        /**
         *  If <code>true</code>, specifies to clip the children to the boundaries of the Viewport. 
         *  If <code>false</code>, the container children extend past the container boundaries, 
         *  regardless of the size specification of the component. 
         */
        ///<summary>
        ///</summary>
        bool ClipAndEnableScrolling { get; set; }
    }
}