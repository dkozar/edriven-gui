using System;
using eDriven.Core.Geom;
using eDriven.Gui.Components;
using eDriven.Gui.Events;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Layout
{
    ///<summary>
    ///</summary>
    public sealed class VerticalLayout : LayoutBase
    {
        #region Helper

        private static float CalculatePercentWidth(ILayoutElement layoutElement, float width)
        {
            float percentWidth = Mathf.Clamp(Mathf.Round((float) (layoutElement.PercentWidth * 0.01f * width)),
                                             LayoutUtil.GetMinBoundsWidth((InvalidationManagerClient)layoutElement),
                                             LayoutUtil.GetMaxBoundsWidth((InvalidationManagerClient) layoutElement));
            return percentWidth < width ? percentWidth : width;
        }

        private static void SizeLayoutElement(ILayoutElement layoutElement,
                                              float width,
                                              HorizontalAlign horizontalAlign,
                                              float restrictedWidth,
                                              float? height,
                                              bool variableRowHeight,
                                              float rowHeight)
        {
            float? newWidth = null;

            // if horizontalAlign is "justify" or "contentJustify", 
            // restrict the width to restrictedWidth.  Otherwise, 
            // size it normally
            if (horizontalAlign == HorizontalAlign.Justify ||
                horizontalAlign == HorizontalAlign.ContentJustify)
            {
                newWidth = restrictedWidth;
            }
            else
            {
                if (null != layoutElement.PercentWidth)
                    newWidth = CalculatePercentWidth(layoutElement, width);
            }

            if (variableRowHeight)
                layoutElement.SetLayoutBoundsSize(newWidth, height);
            else
                layoutElement.SetLayoutBoundsSize(newWidth, rowHeight);
        }

        /**
         *  
         * 
         *  Used only for virtual layout.
         */
        private float? CalculateElementWidth(ILayoutElement elt, float targetWidth, float containerWidth)
        {
           // If percentWidth is specified then the element's width is the percentage
           // of targetWidth clipped to min/maxWidth and to (upper limit) targetWidth.
           float? percentWidth = elt.PercentWidth;
           if (null != percentWidth)
           {
              float width = (float) (percentWidth * 0.01f * targetWidth);
              return Mathf.Min(targetWidth, Mathf.Min(
                  LayoutUtil.GetMaxBoundsWidth((InvalidationManagerClient) elt), 
                  Mathf.Max(LayoutUtil.GetMinBoundsWidth((InvalidationManagerClient)elt), 
                  width
                )));
           }
           switch(_horizontalAlign)
           {
               case HorizontalAlign.Justify: 
                   return targetWidth;
               case HorizontalAlign.ContentJustify:
                   return Mathf.Max(LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) elt), containerWidth);
           }
           return null;  // not constrained
        }

        /**
         *  
         * 
         *  Used only for virtual layout.
         */
        private float CalculateElementX(ILayoutElement elt, float eltWidth, float containerWidth)
        {
           switch(_horizontalAlign)
           {
               case HorizontalAlign.Center: 
                   return Mathf.Round((containerWidth - eltWidth) * 0.5f);
               case HorizontalAlign.Right: 
                   return containerWidth - eltWidth;
           }
           return 0;  // HorizontalAlign.LEFT
        }

        public VerticalLayout()
        {
            DragScrollRegionSizeHorizontal = 0;
        }

        private int _gap = 6;
    
        /**
         *  The vertical space between layout elements, in pixels.
         * 
         *  Note that the gap is only applied between layout elements, so if there's
         *  just one element, the gap has no effect on the layout.
         * 
         *  Default: 6
         */
        ///<summary>
        /// VerticalGap
        ///</summary>
        public int Gap
        {
            get
            {
                return _gap;
            }
            set
            {
                if (_gap == value) 
                    return;

                _gap = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  rowCount
        //----------------------------------

        private int _rowCount = -1;

        /**
         *  Returns the current number of elements in view.
         * 
         *  Default: -1
         */
        public int RowCount
        {
            get
            {
                return _rowCount;
            }
        }

        private void SetRowCount(int value)
        {
            if (_rowCount == value)
                return;

            int oldValue = _rowCount;
            _rowCount = value;
            DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "rowCount", oldValue, value));
        }

        /**
         *  
         * 
         *  This function sets the height of each child
         *  so that the heights add up to <code>height</code>. 
         *  Each child is set to its preferred height
         *  if its percentHeight is zero.
         *  If its percentHeight is a positive number,
         *  the child grows (or shrinks) to consume its
         *  share of extra space.
         *  
         *  The return value is any extra space that's left over
         *  after growing all children to their maxHeight.
         */
        private float DistributeHeight(float width, 
                                          float height, 
                                          float restrictedWidth)
        {
            float spaceToDistribute = height;
            float totalPercentHeight = 0;
            System.Collections.Generic.List<FlexChildInfo> childInfoArray = new System.Collections.Generic.List<FlexChildInfo>();
            FlexChildInfo childInfo;
            ILayoutElement layoutElement;
            
            // rowHeight can be expensive to compute
            float rh = (_variableRowHeight) ? 0 : Mathf.Ceil(RowHeight);
            int count = Target.NumberOfContentChildren;
            int totalCount = count; // number of elements to use in gap calculation
            
            // If the child is flexible, store information about it in the
            // childInfoArray. For non-flexible children, just set the child's
            // width and height immediately.
            for (int index = 0; index < count; index++)
            {
                layoutElement = (ILayoutElement) Target.GetContentChildAt(index);
                if (null == layoutElement || !layoutElement.IncludeInLayout)
                {
                    totalCount--;
                    continue;
                }

                if (null != layoutElement.PercentHeight && _variableRowHeight)
                {
                    totalPercentHeight += (float)layoutElement.PercentHeight;

                    childInfo = new FlexChildInfo
                                    {
                                        LayoutElement = layoutElement,
                                        Percent = (float) layoutElement.PercentHeight,
                                        Min = LayoutUtil.GetMinBoundsHeight((InvalidationManagerClient) layoutElement),
                                        Max = LayoutUtil.GetMaxBoundsHeight((InvalidationManagerClient) layoutElement)
                                    };

                    childInfoArray.Add(childInfo);                
                }
                else
                {
                    SizeLayoutElement(layoutElement, width, _horizontalAlign,
                                   restrictedWidth, null, _variableRowHeight, rh);
                    
                    spaceToDistribute -= Mathf.Ceil(LayoutUtil.GetLayoutBoundsHeight((InvalidationManagerClient) layoutElement));
                } 
            }
            
            if (totalCount > 1)
                spaceToDistribute -= (totalCount-1) * _gap;

            // Distribute the extra space among the flexible children
            if (0 != totalPercentHeight)
            {
                Flex.FlexChildrenProportionally(height,
                                                spaceToDistribute,
                                                totalPercentHeight,
                                                childInfoArray);

                float roundOff = 0;            
                foreach (FlexChildInfo info in childInfoArray)
                {
                    // Make sure the calculated percentages are rounded to pixel boundaries
                    int childSize = (int) Mathf.Round(info.Size + roundOff);
                    roundOff += info.Size - childSize;

                    SizeLayoutElement(info.LayoutElement, width, _horizontalAlign, 
                                   restrictedWidth, childSize,
                                   _variableRowHeight, rh);
                    spaceToDistribute -= childSize;

                    //if (((Component)info.LayoutElement).Id == "test")
                    //{
                    //    Debug.Log("Sized: " + childSize);
                    //}
                    //Debug.Log("Sized: " + childSize);
                }
            }
            return spaceToDistribute;
        }

        /**
         *  
         */
        private HorizontalAlign _horizontalAlign = HorizontalAlign.Left;

        /** 
         *  The horizontal alignment of layout elements.
         *  If the value is <code>"left"</code>, <code>"right"</code>, or <code>"center"</code> then the 
         *  layout element is aligned relative to the container's <code>contentWidth</code> property.
         * 
         *  <p>If the value is <code>"contentJustify"</code>, then the layout element's actual
         *  width is set to the <code>contentWidth</code> of the container.
         *  The <code>contentWidth</code> of the container is the width of the largest layout element. 
         *  If all layout elements are smaller than the width of the container, 
         *  then set the width of all layout elements to the width of the container.</p>
         * 
         *  <p>If the value is <code>"justify"</code> then the layout element's actual width
         *  is set to the container's width.</p>
         *
         *  <p>This property does not affect the layout's measured size.</p>
         *  
         *  Default: "left"
         */
        public HorizontalAlign HorizontalAlign
        {
            get
            {
                return _horizontalAlign;
            }
            set
            {
                if (value == _horizontalAlign) 
                    return;

                _horizontalAlign = value;

                GroupBase layoutTarget = Target;
                if (null != layoutTarget)
                    layoutTarget.InvalidateDisplayList();
            }
        }

        /**
         *  
         */
        private VerticalAlign _verticalAlign = VerticalAlign.Top;
        
        /** 
         *  The vertical alignment of the content relative to the container's height.
         * 
         *  <p>If the value is <code>"bottom"</code>, <code>"middle"</code>, 
         *  or <code>"top"</code> then the layout elements are aligned relative 
         *  to the container's <code>contentHeight</code> property.</p>
         * 
         *  <p>If the value is <code>"contentJustify"</code> then the actual
         *  height of the layout element is set to 
         *  the container's <code>contentHeight</code> property. 
         *  The content height of the container is the height of the largest layout element. 
         *  If all layout elements are smaller than the height of the container, 
         *  then set the height of all the layout elements to the height of the container.</p>
         * 
         *  <p>If the value is <code>"justify"</code> then the actual height
         *  of the layout elements is set to the container's height.</p>
         *
         *  <p>This property has no effect when <code>clipAndEnableScrolling</code> is true
         *  and the <code>contentHeight</code> is greater than the container's height.</p>
         *
         *  <p>This property does not affect the layout's measured size.</p>
         *  
         *  Default: "top"
         */
        public VerticalAlign VerticalAlign
        {
            get
            {
                return _verticalAlign;
            }
            set
            {
                if (value == _verticalAlign) 
                    return;
            
                _verticalAlign = value;
            
                GroupBase layoutTarget = Target;
                if (null != layoutTarget)
                    layoutTarget.InvalidateDisplayList();
            }
        }

        //----------------------------------
        //  paddingLeft
        //----------------------------------

        private float _paddingLeft;
        
        /**
         *  The minimum number of pixels between the container's left edge and
         *  the left edge of the layout element.
         * 
         *  Default: 0
         */
        public float PaddingLeft
        {
            get
            {
                return _paddingLeft;
            }
            set
            {
                if (_paddingLeft == value)
                    return;
                                       
                _paddingLeft = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  paddingRight
        //----------------------------------

        private float _paddingRight;
        
        /**
         *  The minimum number of pixels between the container's right edge and
         *  the right edge of the layout element.
         * 
         *  Default: 0
         */
        public float PaddingRight
        {
            get
            {
                return _paddingRight;
            }
            set
            {
                if (_paddingRight == value)
                    return;
                                       
                _paddingRight = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  paddingTop
        //----------------------------------

        private float _paddingTop;

        /**
         *  Number of pixels between the container's top edge
         *  and the top edge of the first layout element.
         * 
         *  Default: 0
         */
        public float PaddingTop
        {
            get
            {
                return _paddingTop;
            }
            set
            {
                if (_paddingTop == value)
                    return;
                                       
                _paddingTop = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  paddingBottom
        //----------------------------------

        private float _paddingBottom;

        /**
         *  Number of pixels between the container's bottom edge
         *  and the bottom edge of the last layout element.
         * 
         *  Default: 0
         */
        public float PaddingBottom
        {
            get
            {
                return _paddingBottom;
            }
            set
            {
                if (_paddingBottom == value)
                    return;
                                       
                _paddingBottom = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  requestedMinRowCount
        //----------------------------------

        private int _requestedMinRowCount = -1;
        
        /**
         *  The measured height of this layout is large enough to display 
         *  at least <code>requestedMinRowCount</code> layout elements. 
         * 
         *  <p>If <code>RequestedColumnCount</code> is set, then
         *  this property has no effect.</p>
         *
         *  <p>If the actual size of the container has been explicitly set,
         *  then this property has no effect.</p>
         *
         *  Default: -1
         */
        public int RequestedMinRowCount
        {
            get
            {
                return _requestedMinRowCount;
            }
            set
            {
                if (_requestedMinRowCount == value)
                    return;
                                       
                _requestedMinRowCount = value;

                if (null != Target)
                    Target.InvalidateSize();
            }
        }

        //----------------------------------
        //  RequestedColumnCount
        //----------------------------------
        
        private int _requestedRowCount = -1;
        
        /**
         *  The measured size of this layout is tall enough to display 
         *  the first <code>RequestedColumnCount</code> layout elements. 
         * 
         *  <p>If <code>RequestedColumnCount</code> is -1, then the measured
         *  size will be big enough for all of the layout elements.</p>
         * 
         *  <p>If the actual size of the container has been explicitly set,
         *  then this property has no effect.</p>
         * 
         *  Default: -1
         */
        public int RequestedRowCount
        {
            get
            {
                return _requestedRowCount;
            }
            set
            {
                if (_requestedRowCount == value)
                    return;
                
                _requestedRowCount = value;
                
                if (null != Target)
                    Target.InvalidateSize();
            }
        }
        
        //----------------------------------
        //  rowHeight
        //----------------------------------
        
        private float? _rowHeight;

        /**
         *  If <code>variableRowHeight</code> is <code>false</code>, then 
         *  this property specifies the actual height of each child, in pixels.
         * 
         *  <p>If <code>variableRowHeight</code> is <code>true</code>, 
         *  the default, then this property has no effect.</p>
         * 
         *  <p>The default value of this property is the preferred height
         *  of the <code>typicalLayoutElement</code>.</p>
         */
        public float RowHeight
        {
            get
            {
                if (null != _rowHeight)
                    return (float) _rowHeight;
                ILayoutElement elt = TypicalLayoutElement;
                return (null != elt) ? LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) elt) : 0;
            }
            set
            {
                if (_rowHeight == value)
                    return;
                    
                _rowHeight = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  variableRowHeight
        //----------------------------------

        /**
         *  
         */
        private bool  _variableRowHeight = true;

        /**
         *  Specifies whether layout elements are allocated their 
         *  preferred height.
         *  Setting this property to <code>false</code> specifies fixed height rows.
         * 
         *  <p>If <code>false</code>, the actual height of each layout element is 
         *  the value of <code>rowHeight</code>.
         *  Setting this property to <code>false</code> causes the layout to ignore 
         *  the layout elements' <code>percentHeight</code> property.</p>
         * 
         *  Default: true
         */
        public bool VariableRowHeight
        {
            get
            {
                return _variableRowHeight;
            }
            set
            {
                if (value == _variableRowHeight) 
                    return;
                
                _variableRowHeight = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        /**
         *  
         */
        public override bool ClipAndEnableScrolling
        {
            get
            {
                return base.ClipAndEnableScrolling;
            }
            set
            {
                base.ClipAndEnableScrolling = value;

                if (_verticalAlign == VerticalAlign.Middle || _verticalAlign == VerticalAlign.Bottom)
                {
                    GroupBase g = Target;
                    if (null != g)
                        g.InvalidateDisplayList();
                }
            }
        }

        /**
         *  
         *  Fills in the result with preferred and min sizes of the element.
         */
        private void GetElementWidth(ILayoutElement element, bool justify, SizesAndLimit result)
        {
            // Calculate preferred width first, as it's being used to calculate min width
            float elementPreferredWidth = Mathf.Ceil(LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) element));
            
            // Calculate min width
            bool flexibleWidth = null != element.PercentWidth || justify;
            float elementMinWidth = flexibleWidth ? Mathf.Ceil(LayoutUtil.GetMinBoundsWidth((InvalidationManagerClient)element)) : 
                                                         elementPreferredWidth;
            result.PreferredSize = elementPreferredWidth;
            result.MinSize = elementMinWidth;
        }

        /**
         *  
         *  Fills in the result with preferred and min sizes of the element.
         */
        private void GetElementHeight(ILayoutElement element, float? fixedRowHeight, SizesAndLimit result)
        {
            // Calculate preferred height first, as it's being used to calculate min height below
            float elementPreferredHeight = fixedRowHeight ?? Mathf.Ceil(LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) element));
            // Calculate min height
            bool flexibleHeight = null != element.PercentHeight;
            float elementMinHeight = flexibleHeight ? Mathf.Ceil(LayoutUtil.GetMinBoundsHeight((InvalidationManagerClient) element)) : 
                                                           elementPreferredHeight;
            result.PreferredSize = elementPreferredHeight;
            result.MinSize = elementMinHeight;
        }

        //public override Rectangle GetElementBounds(int index)
        //{
        //    //if (!UseVirtualLayout)
        //    return base.GetElementBounds(index);

        //    //var g = Target;
        //    //if (null == g || (index < 0) || (index >= g.NumberOfContentChildren)) 
        //    //    return null;

        //    //return llv.getBounds(index);
        //}

        //public LayoutDirection Direction;
        //public int HorizontalSpacing;
        //public int VerticalSpacing;

        #endregion
        
        ///<summary>
        ///</summary>
        override internal void Measure()
        {
            GroupBase layoutTarget = Target;
            if (null == layoutTarget)
                return;

            SizesAndLimit size = new SizesAndLimit();
            bool justify = _horizontalAlign == HorizontalAlign.Justify;
            int numElements = layoutTarget.NumberOfContentChildren; // How many elements there are in the target
            int numElementsInLayout = numElements;      // How many elements have includeInLayout == true, start off with numElements.
            int requestedRowCount = RequestedRowCount;
            int rowsMeasured = 0;
      
            float preferredHeight = 0; // sum of the elt preferred heights
            float preferredWidth = 0;  // max of the elt preferred widths
            float minHeight = 0;       // sum of the elt minimum heights
            float minWidth = 0;        // max of the elt minimum widths

            float? fixedRowHeight = null;
            if (!_variableRowHeight)
                fixedRowHeight = _rowHeight;  // may query typicalLayoutElement, elt at index=0
        
            ILayoutElement element;
            for (int i = 0; i < numElements; i++)
            {
                element = (ILayoutElement) layoutTarget.GetContentChildAt(i);
                if (null == element || !element.IncludeInLayout)
                {
                    numElementsInLayout--;
                    continue;
                }

                // Can we measure this row height?
                if (RequestedRowCount == -1 || rowsMeasured < RequestedRowCount)
                {
                    GetElementHeight(element, fixedRowHeight, size);
                    preferredHeight += size.PreferredSize;
                    minHeight += size.MinSize;
                    rowsMeasured++;
                }

                //Debug.Log(element + " measured. preferredHeight: " + preferredHeight);

                // Consider the width of each element, inclusive of those outside
                // the RequestedColumnCount range.
                GetElementWidth(element, justify, size);
                preferredWidth = Mathf.Max(preferredWidth, size.PreferredSize);
                minWidth = Mathf.Max(minWidth, size.MinSize);
            }

            // Calculate the total number of rows to measure
            int rowsToMeasure = (_requestedRowCount != -1) ? requestedRowCount : 
                                                                Mathf.Max(_requestedMinRowCount, numElementsInLayout);
            // Do we need to measure more rows?
            if (rowsMeasured < rowsToMeasure)
            {
                // Use the typical element
                element = TypicalLayoutElement;
                if (null != element)
                {
                    // Height
                    GetElementHeight(element, fixedRowHeight, size);
                    preferredHeight += size.PreferredSize * (rowsToMeasure - rowsMeasured);
                    minHeight += size.MinSize * (rowsToMeasure - rowsMeasured);
        
                    // Width
                    GetElementWidth(element, justify, size);
                    preferredWidth = Mathf.Max(preferredWidth, size.PreferredSize);
                    minWidth = Mathf.Max(minWidth, size.MinSize);
                    rowsMeasured = rowsToMeasure;
                }
            }

            // Add gaps
            if (rowsMeasured > 1)
            { 
                float vgap = _gap * (rowsMeasured - 1);
                preferredHeight += vgap;
                minHeight += vgap;
            }

            float hPadding = _paddingLeft + _paddingRight;
            float vPadding = _paddingTop + _paddingBottom;
            
            //layoutTarget.MeasuredHeight = preferredHeight + vPadding;
            //layoutTarget.MeasuredWidth = preferredWidth + hPadding;
            //layoutTarget.MeasuredMinHeight = minHeight + vPadding;
            //layoutTarget.MeasuredMinWidth  = minWidth + hPadding;

            layoutTarget.MeasuredWidth = Mathf.Ceil(preferredWidth + hPadding);
            layoutTarget.MeasuredHeight = Mathf.Ceil(preferredHeight + vPadding);
            layoutTarget.MeasuredMinWidth = Mathf.Ceil(minWidth + hPadding);
            layoutTarget.MeasuredMinHeight = Mathf.Ceil(minHeight + vPadding);  

            /*if (((Component)Target.Parent).Id == "horiz")
                Debug.Log("Measured: " + Target.MeasuredWidth + ", " + Target.MeasuredHeight);*/
        }

        ///<summary>
        ///</summary>
        ///<param name="width"></param>
        ///<param name="height"></param>
        override internal void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            //if (((Component)Target.Owner).Id == "test")
            //    Debug.Log("AbsoluteLayout -> UpdateDisplayList: " + width + ", " + height);

            GroupBase layoutTarget = Target;
            if (null == layoutTarget)
                return;

            if (layoutTarget.NumberOfContentChildren == 0 || width == 0 || height == 0)
            {
                SetRowCount(0);
                SetIndexInView(-1, -1);
                if (layoutTarget.NumberOfContentChildren == 0)
                    layoutTarget.SetContentSize(Mathf.Ceil(_paddingLeft + _paddingRight),
                                                Mathf.Ceil(_paddingTop + _paddingBottom));
                return;
            }

            float targetWidth = Mathf.Max(0, layoutTarget.Width - _paddingLeft - _paddingRight);
            float targetHeight = Mathf.Max(0, layoutTarget.Height - _paddingTop - _paddingBottom);
            
            ILayoutElement layoutElement;
            int count = layoutTarget.NumberOfContentChildren;
            
            // If horizontalAlign is left, we don't need to figure out the contentWidth
            // Otherwise the contentWidth is used to position the element and even size 
            // the element if it's "contentJustify" or "justify".
            float containerWidth = targetWidth;        
            if (_horizontalAlign == HorizontalAlign.ContentJustify ||
               (ClipAndEnableScrolling && (_horizontalAlign == HorizontalAlign.Center ||
                                           _horizontalAlign == HorizontalAlign.Right))) 
            {
                for (int i = 0; i < count; i++)
                {
                    layoutElement = (ILayoutElement) layoutTarget.GetContentChildAt(i);
                    if (null == layoutElement || !layoutElement.IncludeInLayout)
                        continue;

                    float layoutElementWidth;
                    if (null != layoutElement.PercentWidth)
                        layoutElementWidth = CalculatePercentWidth(layoutElement, targetWidth);
                    else
                        layoutElementWidth = LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) layoutElement);
                    
                    containerWidth = Mathf.Max(containerWidth, Mathf.Ceil(layoutElementWidth));
                }
            }

            float excessHeight = DistributeHeight(targetWidth, targetHeight, containerWidth);
            
            // default to left (0)
            float hAlign = 0;
            if (_horizontalAlign == HorizontalAlign.Center)
                hAlign = 0.5f;
            else if (_horizontalAlign == HorizontalAlign.Right)
                hAlign = 1f;
            
            // As the layoutElements are positioned, we'll count how many rows 
            // fall within the layoutTarget's scrollRect
            int visibleRows = 0;
            float minVisibleY = layoutTarget.VerticalScrollPosition;
            float maxVisibleY = minVisibleY + targetHeight;
            
            // Finally, position the layoutElements and find the first/last
            // visible indices, the content size, and the number of 
            // visible elements.    
            float x0 = _paddingLeft;
            float y = _paddingTop;
            float maxX = _paddingLeft;
            float maxY = _paddingTop;
            int firstRowInView = -1;
            int lastRowInView = -1;
            
            // Take verticalAlign into account
            if (excessHeight > 0 || !ClipAndEnableScrolling)
            {
                VerticalAlign vAlign = _verticalAlign;
                if (vAlign == VerticalAlign.Middle)
                {
                    y = _paddingTop + Mathf.Round(excessHeight / 2);   
                }
                else if (vAlign == VerticalAlign.Bottom)
                {
                    y = _paddingTop + excessHeight;   
                }
            }

            for (int index = 0; index < count; index++)
            {
                layoutElement = (ILayoutElement) layoutTarget.GetContentChildAt(index);
                if (null == layoutElement || !layoutElement.IncludeInLayout)
                    continue;
                    
                // Set the layout element's position
                float dx = Mathf.Ceil(LayoutUtil.GetLayoutBoundsWidth((InvalidationManagerClient) layoutElement));
                float dy = Mathf.Ceil(LayoutUtil.GetLayoutBoundsHeight((InvalidationManagerClient) layoutElement));

                float x = x0 + (containerWidth - dx) * hAlign;
                // In case we have HorizontalAlign.CENTER we have to round
                if (hAlign == 0.5f)
                    x = Mathf.Round(x);
                LayoutUtil.SetLayoutBoundsPosition((InvalidationManagerClient) layoutElement, x, y);
                                
                // Update maxX,Y, first,lastVisibleIndex, and y
                maxX = Mathf.Max(maxX, x + dx);
                maxY = Mathf.Max(maxY, y + dy);
                if (!ClipAndEnableScrolling ||
                    ((y < maxVisibleY) && ((y + dy) > minVisibleY)) || 
                    ((dy <= 0) && ((y == maxVisibleY) || (y == minVisibleY))))
                {
                    visibleRows += 1;
                    if (firstRowInView == -1)
                       firstRowInView = lastRowInView = index;
                    else
                       lastRowInView = index;
                }
                y += dy + _gap;
            }
            
            SetRowCount(visibleRows);
            SetIndexInView(firstRowInView, lastRowInView);
            
            // Make sure that if the content spans partially over a pixel to the right/bottom,
            // the content size includes the whole pixel.
            layoutTarget.SetContentSize(Mathf.Ceil(maxX + _paddingRight),
                                        Mathf.Ceil(maxY + _paddingBottom));
        }

        /*/**
         *  Returns: Returns the maximum value for an element's dimension so that the component doesn't
         *  spill out of the container size. Calculations are based on the layout rules.
         *  Pass in unscaledWidth, hCenter, left, right, childX to get a maxWidth value.
         *  Pass in unscaledHeight, vCenter, top, bottom, childY to get a maxHeight value.
         #1#
        static private float MaxSizeToFitIn(float totalSize,
                                            float? center,
                                            float? lowConstraint,
                                            float? highConstraint,
                                            float? position)
        {
            if (null != center)
            {
                // (1) x == (totalSize - childWidth) / 2 + hCenter
                // (2) x + childWidth <= totalSize
                // (3) x >= 0
                //
                // Substitue x in (2):
                // (totalSize - childWidth) / 2 + hCenter + childWidth <= totalSize
                // totalSize - childWidth + 2 * hCenter + 2 * childWidth <= 2 * totalSize
                // 2 * hCenter + childWidth <= totalSize se we get:
                // (3) childWidth <= totalSize - 2 * hCenter
                //
                // Substitute x in (3):
                // (4) childWidth <= totalSize + 2 * hCenter
                //
                // From (3) & (4) above we get:
                // childWidth <= totalSize - 2 * abs(hCenter)

                return totalSize - 2 * Math.Abs((float)center);
            }

            if (null != lowConstraint)
            {
                // childWidth + left <= totalSize
                return (float)(totalSize - lowConstraint);
            }

            if (null != highConstraint)
            {
                // childWidth + right <= totalSize
                return (float)(totalSize - highConstraint);
            }

            // childWidth + childX <= totalSize
            return (float)(totalSize - position);
        }

        private static bool ConstraintsDetermineWidth(ILayoutElement layoutElement)
        {
            return null != layoutElement.PercentWidth ||
                   null != layoutElement.GetConstraintValue("left") &&
                   null != layoutElement.GetConstraintValue("right");
        }

        private static bool ConstraintsDetermineHeight(ILayoutElement layoutElement)
        {
            return null != layoutElement.PercentHeight ||
                   null != layoutElement.GetConstraintValue("top") &&
                   null != layoutElement.GetConstraintValue("bottom");
        }*/

        private int _firstIndexInView = -1;

        /**
         *  The index of the first layout element that is part of the 
         *  layout and within the layout target's scroll rectangle, or -1 
         *  if nothing has been displayed yet.
         *  
         *  <p>"Part of the layout" means that the element is non-null
         *  and that its <code>includeInLayout</code> property is <code>true</code>.</p>
         * 
         *  <p>Note that the layout element may only be partially in view.</p>
         */
        internal int FirstIndexInView
        {
            get { return _firstIndexInView; }
        }

        private int _lastIndexInView = -1;

        /**
         *  The index of the last row that's part of the layout and within
         *  the container's scroll rectangle, or -1 if nothing has been displayed yet.
         * 
         *  <p>"Part of the layout" means that the child is non-null
         *  and that its <code>includeInLayout</code> property is <code>true</code>.</p>
         * 
         *  <p>Note that the row may only be partially in view.</p>
         */
        internal int LastIndexInView
        {
            get { return _lastIndexInView; }
        }

        /**
         *  Sets the <code>firstIndexInView</code> and <code>lastIndexInView</code>
         *  properties and dispatches a <code>"indexInViewChanged"</code>
         *  event.  
         * 
         *  Param: firstIndex The new value for firstIndexInView.
         *  Param: lastIndex The new value for lastIndexInView.
         */
        private void SetIndexInView(int firstIndex, int lastIndex)
        {
            if ((_firstIndexInView == firstIndex) && (_lastIndexInView == lastIndex))
                return;
            
            _firstIndexInView = firstIndex;
            _lastIndexInView = lastIndex;
            DispatchEvent(new Event("indexInViewChanged"));
        }

        /**
         *   
         */
        override internal int GetNavigationDestinationIndex(int currentIndex, NavigationUnit navigationUnit, bool arrowKeysWrapFocus)
        {
            if (null == Target || Target.NumberOfContentChildren < 1)
                return -1;

            int maxIndex = Target.NumberOfContentChildren - 1;

            // Special case when nothing was previously selected
            if (currentIndex == -1)
            {
                if (navigationUnit == NavigationUnit.Up)
                    return arrowKeysWrapFocus ? maxIndex : -1;

                if (navigationUnit == NavigationUnit.Down)
                    return 0;
            }

            currentIndex = Math.Max(0, Math.Min(maxIndex, currentIndex));

            int newIndex;
            Rectangle bounds;
            float y;

            switch (navigationUnit)
            {
                case NavigationUnit.Up:
                    {
                        if (arrowKeysWrapFocus && currentIndex == 0)
                            newIndex = maxIndex;
                        else
                            newIndex = currentIndex - 1;
                        break;
                    }

                case NavigationUnit.Down:
                    {
                        if (arrowKeysWrapFocus && currentIndex == maxIndex)
                            newIndex = 0;
                        else
                            newIndex = currentIndex + 1;
                        break;
                    }

                case NavigationUnit.PageUp:
                    // Find the first fully visible element
                    var firstVisible = FirstIndexInView;
                    var firstFullyVisible = firstVisible;
                    if (FractionOfElementInView(firstFullyVisible) < 1)
                        firstFullyVisible += 1;

                    // Is the current element in the middle of the viewport?
                    if (firstFullyVisible < currentIndex && currentIndex <= LastIndexInView)
                        newIndex = firstFullyVisible;
                    else
                    {
                        // Find an element that's one page up
                        if (currentIndex == firstFullyVisible || currentIndex == firstVisible)
                        {
                            // currentIndex is visible, we can calculate where the scrollRect top
                            // would end up if we scroll by a page                    
                            y = GetVerticalScrollPositionDelta(NavigationUnit.PageUp) + GetScrollRect().Top;
                        }
                        else
                        {
                            // currentIndex is not visible, just find an element a page up from currentIndex
                            y = GetElementBounds(currentIndex).Bottom - GetScrollRect().Height;
                        }

                        // Find the element after the last element that spans above the y position
                        newIndex = currentIndex - 1;
                        while (0 <= newIndex)
                        {
                            bounds = GetElementBounds(newIndex);
                            if (null != bounds && bounds.Top < y)
                            {
                                // This element spans the y position, so return the next one
                                newIndex = Math.Min(currentIndex - 1, newIndex + 1);
                                break;
                            }
                            newIndex--;
                        }
                    }
                    break;

                case NavigationUnit.PageDown:
                    // Find the last fully visible element:
                    var lastVisible = LastIndexInView;
                    var lastFullyVisible = lastVisible;
                    if (FractionOfElementInView(lastFullyVisible) < 1)
                        lastFullyVisible -= 1;

                    // Is the current element in the middle of the viewport?
                    if (FirstIndexInView <= currentIndex && currentIndex < lastFullyVisible)
                        newIndex = lastFullyVisible;
                    else
                    {
                        // Find an element that's one page down
                        if (currentIndex == lastFullyVisible || currentIndex == lastVisible)
                        {
                            // currentIndex is visible, we can calculate where the scrollRect bottom
                            // would end up if we scroll by a page                    
                            y = GetVerticalScrollPositionDelta(NavigationUnit.PageDown) + GetScrollRect().Bottom;
                        }
                        else
                        {
                            // currentIndex is not visible, just find an element a page down from currentIndex
                            y = GetElementBounds(currentIndex).Top + GetScrollRect().Height;
                        }

                        // Find the element before the first element that spans below the y position
                        newIndex = currentIndex + 1;
                        while (newIndex <= maxIndex)
                        {
                            bounds = GetElementBounds(newIndex);
                            if (null != bounds && bounds.Bottom > y)
                            {
                                // This element spans the y position, so return the previous one
                                newIndex = Math.Max(currentIndex + 1, newIndex - 1);
                                break;
                            }
                            newIndex++;
                        }
                    }
                    break;
                default:
                    return base.GetNavigationDestinationIndex(currentIndex, navigationUnit, arrowKeysWrapFocus);
            }
            return Math.Max(0, Math.Min(maxIndex, newIndex));
        }

        /**
         *  Returns 1.0 if the specified index is completely in view, 0.0 if
         *  it's not, or a value between 0.0 and 1.0 that represents the percentage 
         *  of the if the index that is partially in view.
         * 
         *  <p>An index is "in view" if the corresponding non-null layout element is 
         *  within the vertical limits of the container's <code>scrollRect</code>
         *  and included in the layout.</p>
         *  
         *  <p>If the specified index is partially within the view, the 
         *  returned value is the percentage of the corresponding
         *  layout element that's visible.</p>
         *
         *  Param: index The index of the row.
         * 
         *  Returns: The percentage of the specified element that's in view.
         *  Returns 0.0 if the specified index is invalid or if it corresponds to
         *  null element, or a ILayoutElement for which 
         *  the <code>includeInLayout</code> property is <code>false</code>.
         */
        internal float FractionOfElementInView(int index) 
        {
            var g = Target;
            if (null == g)
                return 0.0f;
                
            if ((index < 0) || (index >= g.NumberOfContentChildren))
               return 0.0f;
               
            if (!ClipAndEnableScrolling)
                return 1.0f;

               
            var r0 = FirstIndexInView;  
            var r1 = LastIndexInView;
            
            // outside the visible index range
            if ((r0 == -1) || (r1 == -1) || (index < r0) || (index > r1))
                return 0.0f;

            // within the visible index range, but not first or last            
            if ((index > r0) && (index < r1))
                return 1.0f;

            // get the layout element's Y and Height
            float eltY;
            float eltHeight;
            //if (UseVirtualLayout)
            //{
            //    eltY = llv.start(index);
            //    eltHeight = llv.getMajorSize(index);
            //}
            //else 
            //{
            var elt = g.GetContentChildAt(index); // ILayoutElement
            if (null == elt || !elt.IncludeInLayout)
                return 0.0f;
            eltY = LayoutUtil.GetLayoutBoundsY((InvalidationManagerClient) elt); // elt.getLayoutBoundsY();
            eltHeight = LayoutUtil.GetLayoutBoundsHeight((InvalidationManagerClient) elt); // elt.getLayoutBoundsHeight();
            //}
                
            // So, index is either the first or last row in the scrollRect
            // and potentially partially visible.
            //   y0,y1 - scrollRect top,bottom edges
            //   iy0, iy1 - layout element top,bottom edges
            var y0 = g.VerticalScrollPosition;
            var y1 = y0 + g.Height;
            var iy0 = eltY;
            var iy1 = iy0 + eltHeight;
            if (iy0 >= iy1)  // element has 0 or negative height
                return 1.0f;
            if ((iy0 >= y0) && (iy1 <= y1))
                return 1.0f;
            return (Math.Min(y1, iy1) - Math.Max(y0, iy0)) / (iy1 - iy0);
        }

        // TEMP commented
        /*override internal Rectangle GetElementBoundsAboveScrollRect(Rectangle scrollRect)
        {
            return FindLayoutElementBounds(Target, FirstIndexInView, -1, scrollRect);
        }*/

        // TEMP commented
        /*override internal Rectangle GetElementBoundsBelowScrollRect(Rectangle scrollRect)
        {
            return FindLayoutElementBounds(Target, LastIndexInView, 1, scrollRect);
        }*/

        /**
         *  
         * 
         *  Returns the index of the first non-null includeInLayout element, 
         *  beginning with the element at index i.  
         * 
         *  Returns -1 if no such element can be found.
         */
        private static int FindLayoutElementIndex(GroupBase g, int i, int dir)
        {
            var n = g.NumberOfContentChildren;
            while((i >= 0) && (i < n))
            {
               var element = g.GetContentChildAt(i);
               if (null != element && element.IncludeInLayout)
               {
                   return i;      
               }
               i += dir;
            }
            return -1;
        }

        /**
         *  
         * 
         *  Updates the first,lastIndexInView properties per the new
         *  scroll position.
         */
        override internal void ScrollPositionChanged()
        {
            base.ScrollPositionChanged();
        
            var g = Target;
            if (null != g)
                return;     

            var n = g.NumberOfContentChildren - 1;
            if (n < 0) 
            {
                SetIndexInView(-1, -1);
                return;
            }
        
            var scrollR = GetScrollRect();
            if (null == scrollR)
            {
                SetIndexInView(0, n);
                return;    
            }
        
            // We're going to use findIndexAt to find the index of 
            // the elements that overlap the top and bottom edges of the scrollRect.
            // Values that are exactly equal to scrollRect.bottom aren't actually
            // rendered, since the top,bottom interval is only half open.
            // To account for that we back away from the bottom edge by a
            // hopefully infinitesimal amount.
        
            var y0 = scrollR.Top;
            var y1 = scrollR.Bottom - 0.0001f;
            if (y1 <= y0)
            {
                SetIndexInView(-1, -1);
                return;
            }

            int i0;
            int i1;
            /*if (useVirtualLayout)
            {
                i0 = llv.indexOf(y0);
                i1 = llv.indexOf(y1);
            }
            else
            {*/
                i0 = FindIndexAt(y0 + Gap, Gap, g, 0, n);
                i1 = FindIndexAt(y1, Gap, g, 0, n);
            /*}*/
        
            // Special case: no element overlaps y0, is index 0 visible?
            if (i0 == -1)
            {   
                var index0 = FindLayoutElementIndex(g, 0, 1);
                if (index0 != -1)
                {
                    var element0 = (InvalidationManagerClient) g.GetContentChildAt(index0);
                    var element0Y = LayoutUtil.GetLayoutBoundsY(element0);
                    var elementHeight = LayoutUtil.GetLayoutBoundsHeight(element0);
                    if ((element0Y < y1) && ((element0Y + elementHeight) > y0))
                        i0 = index0;
                }
            }

            // Special case: no element overlaps y1, is index n visible?
            if (i1 == -1)
            {
                var index1 = FindLayoutElementIndex(g, n, -1);
                if (index1 != -1)
                {
                    var element1 = (InvalidationManagerClient) g.GetContentChildAt(index1); 
                    var element1Y = LayoutUtil.GetLayoutBoundsY(element1);
                    var element1Height = LayoutUtil.GetLayoutBoundsHeight(element1);       
                    if ((element1Y < y1) && ((element1Y + element1Height) > y0))
                        i1 = index1;
                }
            }
        
            /*if (useVirtualLayout)
                g.InvalidateDisplayList();*/
                
            SetIndexInView(i0, i1);
        }

        /**
         *  
         * 
         *  Binary search for the first layout element that contains y.  
         * 
         *  This function considers both the element's actual bounds and 
         *  the gap that follows it to be part of the element.  The search 
         *  covers index i0 through i1 (inclusive).
         *  
         *  This function is intended for variable height elements.
         * 
         *  Returns the index of the element that contains y, or -1.
         */
        private static int FindIndexAt(float y, int gap, GroupBase g, int i0, int i1)
        {
            var index = (i0 + i1) / 2;
            var element = (InvalidationManagerClient) g.GetContentChildAt(index);     
            var elementY = LayoutUtil.GetLayoutBoundsY(element);
            var elementHeight = LayoutUtil.GetLayoutBoundsHeight(element);
            // TBD: deal with null element, includeInLayout false.
            if ((y >= elementY) && (y < elementY + elementHeight + gap))
                return index;
            if (i0 == i1)
                return -1;
            if (y < elementY)
                return FindIndexAt(y, gap, g, i0, Math.Max(i0, index-1));
            return FindIndexAt(y, gap, g, Math.Min(index+1, i1), i1);
        } 

        /**
         *  
         * 
         *  Returns the actual position/size Rectangle of the first partially 
         *  visible or not-visible, non-null includeInLayout element, beginning
         *  with the element at index i, searching in direction dir (dir must
         *  be +1 or -1).   The last argument is the GroupBase scrollRect, it's
         *  guaranteed to be non-null.
         * 
         *  Returns null if no such element can be found.
         */
        /*private Rectangle FindLayoutElementBounds(GroupBase g, int i, int dir, Rectangle r)
        {
            var n = g.NumberOfContentChildren;

            if (FractionOfElementInView(i) >= 1)
            {
                // Special case: if we hit the first/last element, 
                // then return the area of the padding so that we
                // can scroll all the way to the start/end.
                i += dir;
                if (i < 0)
                    return new Rectangle(0, 0, 0, PaddingTop);
                if (i >= n)
                    return new Rectangle(0, GetElementBounds(n-1).Bottom, 0, PaddingBottom);
            }

            while((i >= 0) && (i < n))
            {
               var elementR = GetElementBounds(i);
               // Special case: if the scrollRect r _only_ contains
               // elementR, then if we're searching up (dir == -1),
               // and elementR's top edge is visible, then try again
               // with i-1.   Likewise for dir == +1.
               if (null != elementR)
               {
                   var overlapsTop = (dir == -1) && (elementR.Top == r.Top) && (elementR.Bottom >= r.Bottom);
                   var overlapsBottom = (dir == +1) && (elementR.Bottom == r.Bottom) && (elementR.Top <= r.Top);
                   if (!(overlapsTop || overlapsBottom))             
                       return elementR;
               }
               i += dir;
            }
            return null;
        }*/
    }
}