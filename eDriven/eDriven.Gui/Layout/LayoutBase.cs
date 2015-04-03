using System;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Components;

namespace eDriven.Gui.Layout
{
    public class LayoutBase : LazyEventDispatcher
    {
        ///<summary>
        ///</summary>
// ReSharper disable InconsistentNaming
        public const string LAYOUT_CHANGED = "layoutChanged";
// ReSharper restore InconsistentNaming

        private GroupBase _target;
        internal GroupBase Target // TEMP internal
        {
            get
            {
                return _target;
            }
            set
            {
                if (_target == value)
                    return;
                ClearVirtualLayoutCache();
                _target = value;
            }
        }
        
        private float _horizontalScrollPosition;

        internal float HorizontalScrollPosition // TEMP internal
        {
            get
            {
                return _horizontalScrollPosition;
            }
            set {
                if (value == _horizontalScrollPosition)
                    return;

                _horizontalScrollPosition = value;
                ScrollPositionChanged();
            }
        }

        internal virtual void ScrollPositionChanged() // TEMP internal (protected)
        {
            GroupBase g = Target;
            if (null == g)
                return;

            UpdateScrollRect(g.Width, g.Height);
        }

        private float _verticalScrollPosition;

        internal float VerticalScrollPosition // TEMP internal
        {
            get
            {
                return _verticalScrollPosition;
            }
            set
            {
                if (value == _verticalScrollPosition)
                    return;

                _verticalScrollPosition = value;
                ScrollPositionChanged();
            }
        }

        private bool _clipAndEnableScrolling;

        public virtual bool ClipAndEnableScrolling
        {
            get { return _clipAndEnableScrolling; }
            set { 
                if (value == _clipAndEnableScrolling) 
                    return;
            
                _clipAndEnableScrolling = value;
                GroupBase g = Target;
                if (null != g)
                    UpdateScrollRect(g.Width, g.Height);
            }
        }

        //----------------------------------
        //  typicalLayoutElement
        //----------------------------------

        private ILayoutElement _typicalLayoutElement;

        /**
         *  Used by layouts when fixed row/column sizes are requested but
         *  a specific size isn't specified.
         *  Used by virtual layouts to estimate the size of layout elements
         *  that have not been scrolled into view.
         *
         *  <p>This property references a component that Flex uses to 
         *  define the height of all container children, 
         *  as the following example shows:</p>
         * 
         *  <pre>
         *  &lt;s:Group&gt;
         *    &lt;s:layout&gt;
         *      &lt;s:VerticalLayout variableRowHeight="false"
         *          typicalLayoutElement="{b3}"/&gt; 
         *    &lt;/s:layout&gt;
         *    &lt;s:Button id="b1" label="Button 1"/&gt;
         *    &lt;s:Button id="b2" label="Button 2"/&gt;
         *    &lt;s:Button id="b3" label="Button 3" fontSize="36"/&gt;
         *    &lt;s:Button id="b4" label="Button 4" fontSize="24"/&gt;
         *  &lt;/s:Group&gt;</pre>
         * 
         *  <p>If this property has not been set and the target is non-null 
         *  then the target's first layout element is cached and returned.</p>
         * 
         *  <p>The default value is the target's first layout element.</p>
         *
         *  Default: null
         */
        internal ILayoutElement TypicalLayoutElement // TEMP internal
        {
            get
            {
                if (null == _typicalLayoutElement && null != Target && Target.NumberOfChildren > 0)
                    _typicalLayoutElement = (ILayoutElement) Target.GetChildAt(0);
                return _typicalLayoutElement;
            }
            set
            {
                if (_typicalLayoutElement == value)
                    return;

                _typicalLayoutElement = value;
                GroupBase g = Target;
                if (null != g)
                    g.InvalidateSize();
            }
        }

        internal virtual void Measure() // TEMP internal
        {
            // override in descendant
        }

        internal virtual void UpdateDisplayList(float width, float height) // TEMP internal
        {
            // override in descendant
        }

        internal void UpdateScrollRect(float width, float height) // TEMP internal
        {
            //if (Target.Id == "view")
            //    Debug.Log("LayoutBase.UpdateScrollRect: " + width + ", " + height);

            GroupBase g = Target;
            if (null == g)
                return;

            //if (Target.Id == "contentGroup")
            //    Debug.Log("ClipAndEnableScrolling: " + ClipAndEnableScrolling);

            if (ClipAndEnableScrolling)
            {
                float hsp = _horizontalScrollPosition;
                float vsp = _verticalScrollPosition;

                //if (Target.Id == "view")
                //    Debug.Log("g.ScrollRect clipped " + new Rectangle(hsp, vsp, width, height));

                g.ScrollRect = new Rectangle(hsp, vsp, width, height);
            }
            else {
                //g.ScrollRect = null;

                //if (Target.Id == "view")
                //    Debug.Log("g.ScrollRect " + new Rectangle(0, 0, width, height));

                g.ScrollRect = null; // new Rectangle(0, 0, width, height);
            }
        }

        internal Rectangle GetScrollRect() // TEMP internal (protected)
        {
            GroupBase g = Target;
            if (null == g || !g.ClipAndEnableScrolling)
                return null;     
            float vsp = g.VerticalScrollPosition;
            float hsp = g.HorizontalScrollPosition;
            return new Rectangle(hsp, vsp, g.Width, g.Height);
        }

        /// <summary>
        /// Returns the amount to add to the viewport's current horizontalScrollPosition to scroll by the requested scrolling unit.
        /// </summary>
        /// <param name="navigationUnit"></param>
        /// <returns></returns>
        internal float GetHorizontalScrollPositionDelta(NavigationUnit navigationUnit) // TEMP internal
        {
            GroupBase g = Target;
            if (null == g)
                return 0;     

            Rectangle scrollRect = GetScrollRect();
            if (null == scrollRect)
                return 0;
                
            // Special case: if the scrollRect's origin is 0,0 and it's bigger 
            // than the target, then there's no where to scroll to
            if ((scrollRect.X == 0) && (scrollRect.Width >= g.ContentWidth))
                return 0;  

            // maxDelta is the horizontalScrollPosition delta required 
            // to scroll to the END and minDelta scrolls to HOME. 
            float maxDelta = g.ContentWidth - scrollRect.Right; //Right;
            float minDelta = -scrollRect.Left; //Left;
            Rectangle getElementBounds;
            switch(navigationUnit)
            {
                case NavigationUnit.Left:
                case NavigationUnit.PageLeft:
                    // Find the bounds of the first non-fully visible element
                    // to the left of the scrollRect.
                    getElementBounds = GetElementBoundsLeftOfScrollRect(scrollRect);
                    break;

                case NavigationUnit.Right:
                case NavigationUnit.PageRight:
                    // Find the bounds of the first non-fully visible element
                    // to the right of the scrollRect.
                    getElementBounds = GetElementBoundsRightOfScrollRect(scrollRect);
                    break;

                case NavigationUnit.Home: 
                    return minDelta;
                    
                case NavigationUnit.End: 
                    return maxDelta;
                    
                default:
                    return 0;
            }
            
            if (null == getElementBounds)
                return 0;

            float delta = 0f;
            switch (navigationUnit)
            {
                case NavigationUnit.Left:
                    // Snap the left edge of element to the left edge of the scrollRect.
                    // The element is the the first non-fully visible element left of the scrollRect.
                    delta = Math.Max(getElementBounds.Left - scrollRect.Left, -scrollRect.Width);
                    break;    
                case NavigationUnit.Right:
                    // Snap the right edge of the element to the right edge of the scrollRect.
                    // The element is the the first non-fully visible element right of the scrollRect.
                    delta = Math.Min(getElementBounds.Right - scrollRect.Right, scrollRect.Width);
                    break;    
                case NavigationUnit.PageLeft:
                    {
                        // Snap the right edge of the element to the right edge of the scrollRect.
                        // The element is the the first non-fully visible element left of the scrollRect. 
                        delta = getElementBounds.Right - scrollRect.Right;
                    
                        // Special case: when an element is wider than the scrollRect,
                        // we want to snap its left edge to the left edge of the scrollRect.
                        // The delta will be limited to the width of the scrollRect further below.
                        if (delta >= 0)
                            delta = Math.Max(getElementBounds.Left - scrollRect.Left, -scrollRect.Width);  
                    }
                    break;
                case NavigationUnit.PageRight:
                    {
                        // Align the left edge of the element to the left edge of the scrollRect.
                        // The element is the the first non-fully visible element right of the scrollRect.
                        delta = getElementBounds.Left - scrollRect.Left;
                    
                        // Special case: when an element is wider than the scrollRect,
                        // we want to snap its right edge to the right edge of the scrollRect.
                        // The delta will be limited to the width of the scrollRect further below.
                        if (delta <= 0)
                            delta = Math.Min(getElementBounds.Right - scrollRect.Right, scrollRect.Width);
                    }
                    break;
            }

            // Makse sure we don't get out of bounds. Also, don't scroll 
            // by more than the scrollRect width at a time.
            return Math.Min(maxDelta, Math.Max(minDelta, delta * (
                this is TileLayout || navigationUnit == NavigationUnit.PageUp || navigationUnit == NavigationUnit.PageDown || navigationUnit == NavigationUnit.PageLeft || navigationUnit == NavigationUnit.PageRight ?
                1 : StepSize))); // temp dirty fix
        }

        /// <summary>
        /// Returns the amount to add to the viewport's current verticalScrollPositionto scroll by the requested scrolling unit.
        /// </summary>
        /// <param name="navigationUnit"></param>
        /// <returns></returns>
        internal float GetVerticalScrollPositionDelta(NavigationUnit navigationUnit) // TEMP internal
        {
            GroupBase g = Target;
            if (null == g)
                return 0;     

            Rectangle scrollRect = GetScrollRect();
            if (null == scrollRect)
                return 0;

            // Special case: if the scrollRect's origin is 0,0 and it's bigger 
            // than the target, then there's no where to scroll to
            if ((scrollRect.Y == 0) && (scrollRect.Height >= g.ContentHeight))
                return 0;

            // maxDelta is the horizontalScrollPosition delta required 
            // to scroll to the END and minDelta scrolls to HOME. 
            float maxDelta = g.ContentHeight - scrollRect.Bottom; // Bottom
            float minDelta = -scrollRect.Top; // Top
            Rectangle getElementBounds;
            switch(navigationUnit)
            {
                case NavigationUnit.Up:
                case NavigationUnit.PageUp:
                    // Find the bounds of the first non-fully visible element
                    // that spans right of the scrollRect.
                    getElementBounds = GetElementBoundsAboveScrollRect(scrollRect);
                    break;

                case NavigationUnit.Down:
                case NavigationUnit.PageDown:
                    // Find the bounds of the first non-fully visible element
                    // that spans below the scrollRect.
                    getElementBounds = GetElementBoundsBelowScrollRect(scrollRect);
                    break;

                case NavigationUnit.Home: 
                    return minDelta;

                case NavigationUnit.End: 
                    return maxDelta;

                default:
                    return 0;
            }

            if (null == getElementBounds)
                return 0;

            float delta = 0;
            switch (navigationUnit)
            {
                case NavigationUnit.Up:
                    // Snap the top edge of element to the top edge of the scrollRect.
                    // The element is the the first non-fully visible element above the scrollRect.
                    delta = Math.Max(getElementBounds.Top - scrollRect.Top, -scrollRect.Height);
                    break;    
                case NavigationUnit.Down:
                    // Snap the bottom edge of the element to the bottom edge of the scrollRect.
                    // The element is the the first non-fully visible element below the scrollRect.
                    delta = Math.Min(getElementBounds.Bottom - scrollRect.Bottom, scrollRect.Height);
                    break;    
                case NavigationUnit.PageUp:
                    {
                        // Snap the bottom edge of the element to the bottom edge of the scrollRect.
                        // The element is the the first non-fully visible element below the scrollRect. 
                        delta = getElementBounds.Bottom - scrollRect.Bottom;
                    
                        // Special case: when an element is taller than the scrollRect,
                        // we want to snap its top edge to the top edge of the scrollRect.
                        // The delta will be limited to the height of the scrollRect further below.
                        if (delta >= 0)
                            delta = Math.Max(getElementBounds.Top - scrollRect.Top, -scrollRect.Height);  
                    }
                    break;
                case NavigationUnit.PageDown:
                    {
                        // Align the top edge of the element to the top edge of the scrollRect.
                        // The element is the the first non-fully visible element below the scrollRect.
                        delta = getElementBounds.Top - scrollRect.Top;
                    
                        // Special case: when an element is taller than the scrollRect,
                        // we want to snap its bottom edge to the bottom edge of the scrollRect.
                        // The delta will be limited to the height of the scrollRect further below.
                        if (delta <= 0)
                            delta = Math.Min(getElementBounds.Bottom - scrollRect.Bottom, scrollRect.Height);
                    }
                    break;
            }

            //Debug.Log("delta: " + delta + "; minDelta: " + minDelta + ", maxDelta: " + maxDelta);
            return Math.Min(maxDelta, Math.Max(minDelta, delta * (
                this is TileLayout || navigationUnit == NavigationUnit.PageUp || navigationUnit == NavigationUnit.PageDown ? 
                1 : StepSize))); // temp dirty fix
        }

        /// <summary>
        /// Step size used when scrolling with mouse wheel or increase/decrease keys
        /// </summary>
        public int StepSize = 32; // my temp fix!

        internal virtual Rectangle GetElementBoundsLeftOfScrollRect(Rectangle scrollRect) // TEMP internal (protected)
        {
            return new Rectangle { Left = scrollRect.Left - 1, Right = scrollRect.Left };
        }

        internal virtual Rectangle GetElementBoundsRightOfScrollRect(Rectangle scrollRect) // TEMP internal (protected)
        {
            return new Rectangle {Left = scrollRect.Right , Right = scrollRect.Right + 1};
        }

        internal virtual Rectangle GetElementBoundsAboveScrollRect(Rectangle scrollRect) // TEMP internal (protected)
        {
            return new Rectangle { Top = scrollRect.Top - 1, Bottom = scrollRect.Top };
        }

        internal virtual Rectangle GetElementBoundsBelowScrollRect(Rectangle scrollRect) // TEMP internal (protected)
        {
            return new Rectangle { Top = scrollRect.Bottom, Bottom = scrollRect.Bottom + 1 };
        }

        internal float DragScrollRegionSizeHorizontal = 20; // TEMP internal
        internal float DragScrollRegionSizeVertical = 20; // TEMP internal

        internal void InvalidateTargetSizeAndDisplayList() // TEMP internal (protected)
        {
            GroupBase g = Target;
            if (null == g)
                return;

            g.InvalidateSize();
            g.InvalidateDisplayList();
        }

        /**
         *  When <code>useVirtualLayout</code> is <code>true</code>, 
         *  this method can be used by the layout target
         *  to clear cached layout information when the target changes.   
         * 
         *  <p>For example, when a DataGroup's <code>dataProvider</code> or 
         *  <code>ItemRenderer</code> property changes, cached 
         *  elements sizes become invalid. </p>
         * 
         *  <p>When the <code>useVirtualLayout</code> property changes to <code>false</code>, 
         *  this method is called automatically.</p>
         * 
         *  <p>Subclasses that support <code>useVirtualLayout</code> = <code>true</code> 
         *  must override this method. </p>
         */
        internal virtual void ClearVirtualLayoutCache() // TEMP internal
        {
        }

        /**
         *  Delegation method that determines which item  
         *  to navigate to based on the current item in focus 
         *  and user input in terms of NavigationUnit. This method
         *  is used by subclasses of OldListBase to handle 
         *  keyboard navigation. OldListBase maps user input to
         *  NavigationUnit constants.
         * 
         *  <p>Subclasses can override this method to compute other 
         *  values that are based on the current index and key 
         *  stroke encountered. </p>
         * 
         *  Param: currentIndex The current index of the item with focus.
         * 
         *  Param: navigationUnit The NavigationUnit constant that determines
         *  which item to navigate to next.  
         * 
         *  Param: ArrowKeysWrapFocus If <code>true</code>, using arrow keys to 
         *  navigate within the component wraps when it hits either end.
         * 
         *  Returns: The index of the next item to jump to. Returns -1
         *  when if the layout doesn't recognize the navigationUnit.
         */  
        ///<summary>
        ///</summary>
        ///<param name="currentIndex"></param>
        ///<param name="navigationUnit"></param>
        ///<param name="arrowKeysWrapFocus"></param>
        ///<returns></returns>
        internal virtual int GetNavigationDestinationIndex(int currentIndex, NavigationUnit navigationUnit, bool arrowKeysWrapFocus) // TEMP internal
        {
            if (null == Target || Target.NumberOfChildren < 1)
                return -1; 

            //Sub-classes implement according to their own layout 
            //logic. Common cases handled here. 
            switch (navigationUnit)
            {
                case NavigationUnit.Home:
                    return 0; 

                case NavigationUnit.End:
                    return Target.NumberOfChildren - 1; 

                default:
                    return -1;
            }
        }

        /**
         *  Returns the specified element's layout bounds as a Rectangle or null
         *  if the index is invalid, the corresponding element is null,
         *  <code>includeInLayout=false</code>, 
         *  or if this layout's <code>target</code> property is null.
         *   
         *  <p>Layout subclasses that support <code>useVirtualLayout=true</code> must
         *  override this method to compute a potentially approximate value for
         *  elements that are not in view.</p>
         * 
         *  Param: index Index of the layout element.
         * 
         *  Returns: The specified element's layout bounds.
         */
        internal virtual Rectangle GetElementBounds(int index) // TEMP internal
        {
            GroupBase g = Target;
            if (null == g)
                return null;

             int n = g.NumberOfChildren;
             if ((index < 0) || (index >= n))
                return null;
                
             ILayoutElement elt = (ILayoutElement) g.GetChildAt(index);
             if (null == elt || !elt.IncludeInLayout)
                return null;

             float eltX = LayoutUtil.GetLayoutBoundsX((InvalidationManagerClient) elt);
             float eltY = LayoutUtil.GetLayoutBoundsY((InvalidationManagerClient)elt);
             float eltW = LayoutUtil.GetLayoutBoundsWidth((InvalidationManagerClient)elt);
             float eltH = LayoutUtil.GetLayoutBoundsHeight((InvalidationManagerClient)elt);
             return new Rectangle(eltX, eltY, eltW, eltH);
        }

        /**
         *  Called by the target after a layout element 
         *  has been added and before the target's size and display list are
         *  validated.   
         *  Layouts that cache per element state, like virtual layouts, can 
         *  override this method to update their cache.
         * 
         *  <p>If the target calls this method, it's only guaranteeing that a
         *  a layout element will exist at the specified index at
         *  <code>updateDisplayList()</code> time, for example a DataGroup
         *  with a virtual layout will call this method when an item is added 
         *  to the targets <code>dataProvider</code>.</p>
         * 
         *  <p>By default, this method does nothing.</p>
         * 
         *  Param: index The index of the element that was added.
         */
         ///<summary>
         /// Called by the target after a layout element has been added and before the target's size and display list are validated
         ///</summary>
         ///<param name="index"></param>
         internal virtual void ElementAdded(int index) // TEMP internal
         {
         }

        /**
         *  This method must is called by the target after a layout element 
         *  has been removed and before the target's size and display list are
         *  validated.   
         *  Layouts that cache per element state, like virtual layouts, can 
         *  override this method to update their cache.
         * 
         *  <p>If the target calls this method, it's only guaranteeing that a
         *  a layout element will no longer exist at the specified index at
         *  <code>updateDisplayList()</code> time.
         *  For example, a DataGroup
         *  with a virtual layout calls this method when an item is added to 
         *  the <code>dataProvider</code> property.</p>
         * 
         *  <p>By default, this method does nothing.</p>
         * 
         *  Param: index The index of the element that was added.
         */
         internal void ElementRemoved(int index) // TEMP internal
         {
         }

        /*public DropLocation CalculateDropLocation(Event @event)
        {
            /* Temp #1#
            return new DropLocation();
        }*/

        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
         internal Point GetScrollPositionDeltaToElement(int index) // TEMP internal
        {
            return GetScrollPositionDeltaToElementHelper(index);
        }

        internal Point GetScrollPositionDeltaToElementHelper(int index, float? topOffset = null/* = NaN*/,
                                                               float? bottomOffset = null/* = NaN*/,
                                                               float? leftOffset = null/* = NaN*/,
                                                               float? rightOffset = null/* = NaN*/)
        {

//            Debug.Log(string.Format(@"topOffset: {0}
//bottomOffset: {1}
//leftOffset: {2}
//rightOffset: {3}", topOffset, bottomOffset, leftOffset, rightOffset));

            var elementR = GetElementBounds(index);
            if (null == elementR)
                return null;
            
            var scrollR = GetScrollRect();
            if (null == scrollR || !Target.ClipAndEnableScrolling)
                return null;

//            Debug.Log(string.Format(@"scrollR: {0}
//elementR: {1}
//Contains: {2}", scrollR, elementR, scrollR.Contains(elementR)));

            if (null == topOffset && null == bottomOffset && null == leftOffset && null == rightOffset &&
                (scrollR.Contains(elementR) || elementR.Contains(scrollR)))
                return null;
            
            var dxl = elementR.Left - scrollR.Left;     // left justify element
            var dxr = elementR.Right - scrollR.Right;   // right justify element
            var dyt = elementR.Top - scrollR.Top;       // top justify element
            var dyb = elementR.Bottom - scrollR.Bottom; // bottom justify element

//            Debug.Log(string.Format(@"dxl: {0}
//dxr: {1}
//dyt: {2}
//dyb: {3}", dxl, dxr, dyt, dyb));
            
            // minimize the scroll
            var dx = (Math.Abs(dxl) < Math.Abs(dxr)) ? dxl : dxr;
            var dy = (Math.Abs(dyt) < Math.Abs(dyb)) ? dyt : dyb;
            
            if (null != topOffset)
                dy = (float) (dyt + topOffset);
            else if (null != bottomOffset)
                dy = (float) (dyb - bottomOffset);
            
            if (null != leftOffset)
                dx = (float) (dxl + leftOffset);
            else if (null != rightOffset)
                dx = (float) (dxr - rightOffset);
            
            // scrollR "contains" elementR in just one dimension
            if ((elementR.Left >= scrollR.Left) && (elementR.Right <= scrollR.Right))
                dx = 0;
            /*else */else if ((elementR.Bottom <= scrollR.Bottom) && (elementR.Top >= scrollR.Top)) // Note: "else if" was a bug in Flex
                dy = 0;
            
            // elementR "contains" scrollR in just one dimension
            if ((elementR.Left <= scrollR.Left) && (elementR.Right >= scrollR.Right))
                dx = 0;
            /*else */else if ((elementR.Bottom >= scrollR.Bottom) && (elementR.Top <= scrollR.Top)) // Note: "else if" was a bug in Flex
                dy = 0;
            
            return new Point(dx, dy);
        }
    }
}