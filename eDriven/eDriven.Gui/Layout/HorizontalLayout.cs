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
    public sealed class HorizontalLayout : LayoutBase
    {
        #region Helper

        private static float CalculatePercentHeight(ILayoutElement layoutElement, float height)
        {
            float percentHeight = Mathf.Clamp(Mathf.Round((float)(layoutElement.PercentHeight * 0.01f * height)),
                                             LayoutUtil.GetMinBoundsHeight((InvalidationManagerClient)layoutElement),
                                             LayoutUtil.GetMaxBoundsHeight((InvalidationManagerClient)layoutElement));
            return percentHeight < height ? percentHeight : height;
        }

        private static void SizeLayoutElement(ILayoutElement layoutElement,
                                              float height,
                                              VerticalAlign verticalAlign,
                                              float restrictedHeight,
                                              float? width,
                                              bool variableColumnWidth,
                                              float columnWidth)
        {
            float? newHeight = null;

            // if verticalAlign is "justify" or "contentJustify", 
            // restrict the height to restrictedHeight.  Otherwise, 
            // size it normally
            if (verticalAlign == VerticalAlign.Justify ||
                verticalAlign == VerticalAlign.ContentJustify)
            {
                newHeight = restrictedHeight;
            }
            else
            {
                if (null != layoutElement.PercentHeight)
                    newHeight = CalculatePercentHeight(layoutElement, height);
            }

            if (variableColumnWidth)
                layoutElement.SetLayoutBoundsSize(width, newHeight);
            else
                layoutElement.SetLayoutBoundsSize(columnWidth, newHeight);
        }

        /**
         *  
         * 
         *  Used only for virtual layout.
         */
        //private float? CalculateElementHeight(ILayoutElement elt, float targetHeight, float containerHeight)
        //{
        //   // If percentWidth is specified then the element's width is the percentage
        //   // of targetHeight clipped to min/maxWidth and to (upper limit) targetHeight.
        //   float? percentHeight = elt.PercentHeight;
        //   if (null != percentHeight)
        //   {
        //      float width = (float) (percentHeight * 0.01f * targetHeight);
        //      return Mathf.Min(targetHeight, Mathf.Min(
        //          LayoutUtil.GetMaxBoundsHeight((InvalidationManagerClient) elt), 
        //          Mathf.Max(LayoutUtil.GetMinBoundsHeight((InvalidationManagerClient)elt), 
        //          width
        //        )));
        //   }
        //   switch(_verticalAlign)
        //   {
        //       case VerticalAlign.Justify: 
        //           return targetHeight;
        //       case VerticalAlign.ContentJustify:
        //           return Mathf.Max(LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) elt), containerHeight);
        //   }
        //   return null;  // not constrained
        //}

        /**
         *  
         * 
         *  Used only for virtual layout.
         */
        //private float CalculateElementY(ILayoutElement elt, float eltWidth, float containerWidth)
        //{
        //   switch(_horizontalAlign)
        //   {
        //       case HorizontalAlign.Center: 
        //           return Mathf.Round((containerWidth - eltWidth) * 0.5f);
        //       case HorizontalAlign.Right: 
        //           return containerWidth - eltWidth;
        //   }
        //   return 0;  // HorizontalAlign.LEFT
        //}

        public HorizontalLayout()
        {
            DragScrollRegionSizeVertical = 0;
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
        //  columnCount
        //----------------------------------

        private int _columnCount = -1;
 
        /**
         *  Returns the current number of elements in view.
         * 
         *  Default: -1
         */
        public int ColumnCount
        {
            get
            {
                return _columnCount;
            }
        }

        private void SetColumnCount(int value)
        {
            if (_columnCount == value)
                return;

            int oldValue = _columnCount;
            _columnCount = value;
            DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "columnCount", oldValue, value));
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
        private float DistributeWidth(float width, 
                                          float height, 
                                          float restrictedHeight)
        {
            float spaceToDistribute = width;
            float totalPercentWidth = 0;
            System.Collections.Generic.List<FlexChildInfo> childInfoArray = new System.Collections.Generic.List<FlexChildInfo>();
            FlexChildInfo childInfo;
            float newHeight;
            ILayoutElement layoutElement;
            
            // rowHeight can be expensive to compute
            float cw = (_variableColumnWidth) ? 0 : Mathf.Ceil(ColumnWidth);
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

                if (null != layoutElement.PercentWidth && _variableColumnWidth)
                {
                    totalPercentWidth += (float)layoutElement.PercentWidth;

                    childInfo = new FlexChildInfo
                                    {
                                        LayoutElement = layoutElement,
                                        Percent = (float)layoutElement.PercentWidth,
                                        Min = LayoutUtil.GetMinBoundsWidth((InvalidationManagerClient) layoutElement),
                                        Max = LayoutUtil.GetMaxBoundsWidth(((InvalidationManagerClient) layoutElement))
                                    };

                    childInfoArray.Add(childInfo);                
                }
                else
                {
                    SizeLayoutElement(layoutElement, height, _verticalAlign,
                                   restrictedHeight, null, _variableColumnWidth, cw);
                    
                    spaceToDistribute -= Mathf.Ceil(LayoutUtil.GetLayoutBoundsWidth((InvalidationManagerClient) layoutElement));
                } 
            }
            
            if (totalCount > 1)
                spaceToDistribute -= (totalCount-1) * _gap;

            // Distribute the extra space among the flexible children
            if (0 != totalPercentWidth)
            {
                Flex.FlexChildrenProportionally(width,
                                                spaceToDistribute,
                                                totalPercentWidth,
                                                childInfoArray);

                float roundOff = 0;            
                foreach (FlexChildInfo info in childInfoArray)
                {
                    // Make sure the calculated percentages are rounded to pixel boundaries
                    int childSize = (int) Mathf.Round(info.Size + roundOff);
                    roundOff += info.Size - childSize;

                    SizeLayoutElement(info.LayoutElement, height, _verticalAlign, 
                                   restrictedHeight, childSize,
                                   _variableColumnWidth, cw);
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

        private int _requestedMinColumnCount = -1;
        
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
        public int RequestedMinColumnCount
        {
            get
            {
                return _requestedMinColumnCount;
            }
            set
            {
                if (_requestedMinColumnCount == value)
                    return;
                                       
                _requestedMinColumnCount = value;

                if (null != Target)
                    Target.InvalidateSize();
            }
        }

        //----------------------------------
        //  RequestedColumnCount
        //----------------------------------
        
        private int _requestedColumnCount = -1;
        
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
        public int RequestedColumnCount
        {
            get
            {
                return _requestedColumnCount;
            }
            set
            {
                if (_requestedColumnCount == value)
                    return;
                
                _requestedColumnCount = value;
                
                if (null != Target)
                    Target.InvalidateSize();
            }
        }
        
        //----------------------------------
        //  rowHeight
        //----------------------------------
        
        private float? _columnWidth;

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
        public float ColumnWidth
        {
            get
            {
                if (null != _columnWidth)
                    return (float) _columnWidth;
                ILayoutElement elt = TypicalLayoutElement;
                return (null != elt) ? LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) elt) : 0;
            }
            set
            {
                if (_columnWidth == value)
                    return;
                    
                _columnWidth = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  variableRowHeight
        //----------------------------------

        /**
         *  
         */
        private bool  _variableColumnWidth = true;

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
        public bool VariableColumnWidth
        {
            get
            {
                return _variableColumnWidth;
            }
            set
            {
                if (value == _variableColumnWidth) 
                    return;
                
                _variableColumnWidth = value;
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

                if (_horizontalAlign == HorizontalAlign.Center || _horizontalAlign == HorizontalAlign.Right)
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
        private void GetElementWidth(ILayoutElement element, float? fixedColumnWidth, SizesAndLimit result)
        {
            // Calculate preferred height first, as it's being used to calculate min height below
            float elementPreferredWidth = fixedColumnWidth ?? Mathf.Ceil(LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient)element));
            // Calculate min height
            bool flexibleWidth = null != element.PercentWidth;
            float elementMinWidth = flexibleWidth ? Mathf.Ceil(LayoutUtil.GetMinBoundsWidth((InvalidationManagerClient)element)) :
                                                           elementPreferredWidth;
            result.PreferredSize = elementPreferredWidth;
            result.MinSize = elementMinWidth;
        }

        /**
         *  
         *  Fills in the result with preferred and min sizes of the element.
         */
        private void GetElementHeight(ILayoutElement element, bool justify, SizesAndLimit result)
        {
            // Calculate preferred width first, as it's being used to calculate min width
            float elementPreferredHeight = Mathf.Ceil(LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient)element));
            
            // Calculate min width
            bool flexibleHeight = null != element.PercentHeight || justify;
            float elementMinHeight = flexibleHeight ? Mathf.Ceil(LayoutUtil.GetMinBoundsHeight((InvalidationManagerClient)element)) : 
                                                         elementPreferredHeight;
            result.PreferredSize = elementPreferredHeight;
            result.MinSize = elementMinHeight;
        }

        //public override Rectangle GetElementBounds(int index)
        //{
        //    //if (!UseVirtualLayout)
        //        return base.GetElementBounds(index);

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
            bool justify = _verticalAlign == VerticalAlign.Justify;
            int numElements = layoutTarget.NumberOfContentChildren; // How many elements there are in the target
            int numElementsInLayout = numElements;      // How many elements have includeInLayout == true, start off with numElements.
            int requestedColumnCount = RequestedColumnCount;
            int columnsMeasured = 0;
      
            float preferredHeight = 0; // sum of the elt preferred heights
            float preferredWidth = 0;  // max of the elt preferred widths
            float minHeight = 0;       // sum of the elt minimum heights
            float minWidth = 0;        // max of the elt minimum widths

            float? fixedColumnWidth = null;
            if (!_variableColumnWidth)
                fixedColumnWidth = _columnWidth;  // may query typicalLayoutElement, elt at index=0
        
            ILayoutElement element;
            for (int i = 0; i < numElements; i++)
            {
                element = (ILayoutElement) layoutTarget.GetContentChildAt(i);
                if (null == element || !element.IncludeInLayout)
                {
                    numElementsInLayout--;
                    continue;
                }

                //Debug.Log(element + " measured. preferredHeight: " + preferredHeight);

                // Consider the width of each element, inclusive of those outside
                // the RequestedColumnCount range.
                GetElementHeight(element, justify, size);
                preferredHeight = Mathf.Max(preferredHeight, size.PreferredSize);
                minHeight = Mathf.Max(minHeight, size.MinSize);

                // Can we measure this row height?
                if (RequestedColumnCount == -1 || columnsMeasured < RequestedColumnCount)
                {
                    GetElementWidth(element, fixedColumnWidth, size);
                    preferredWidth += size.PreferredSize;
                    minWidth += size.MinSize;
                    columnsMeasured++;
                }
            }

            // Calculate the total number of rows to measure
            int columnsToMeasure = (_requestedColumnCount != -1) ? requestedColumnCount : 
                                                                Mathf.Max(_requestedMinColumnCount, numElementsInLayout);
            // Do we need to measure more rows?
            if (columnsMeasured < columnsToMeasure)
            {
                // Use the typical element
                element = TypicalLayoutElement;
                if (null != element)
                {
                    // Height
                    GetElementHeight(element, justify, size);
                    preferredHeight = Mathf.Max(preferredHeight, size.PreferredSize);
                    minHeight = Mathf.Max(minHeight, size.MinSize);
                    
                    // Width
                    GetElementWidth(element, fixedColumnWidth, size);
                    preferredWidth += size.PreferredSize * (columnsToMeasure - columnsMeasured);
                    minWidth += size.MinSize * (columnsToMeasure - columnsMeasured);
                    columnsMeasured = columnsToMeasure;
                }
            }

            // Add gaps
            if (columnsMeasured > 1)
            {
                float hgap = _gap * (columnsMeasured - 1);
                preferredWidth += hgap;
                minWidth += hgap;
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

            if ((layoutTarget.NumberOfContentChildren == 0) || (width == 0) || (height == 0))
            {
                SetColumnCount(0);
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
            float containerHeight = targetHeight;
            if (_verticalAlign == VerticalAlign.ContentJustify ||
               (ClipAndEnableScrolling && (_verticalAlign == VerticalAlign.Middle ||
                                           _verticalAlign == VerticalAlign.Bottom))) 
            {
                for (int i = 0; i < count; i++)
                {
                    layoutElement = (ILayoutElement) layoutTarget.GetContentChildAt(i);
                    if (null == layoutElement || !layoutElement.IncludeInLayout)
                        continue;

                    float layoutElementHeight;
                    if (null != layoutElement.PercentHeight)
                        layoutElementHeight = CalculatePercentHeight(layoutElement, targetHeight);
                    else
                        layoutElementHeight = LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) layoutElement);

                    containerHeight = Mathf.Max(containerHeight, Mathf.Ceil(layoutElementHeight));
                }
            }

            float excessWidth = DistributeWidth(targetWidth, targetHeight, containerHeight);
            
            // default to left (0)
            float vAlign = 0;
            if (_verticalAlign == VerticalAlign.Middle)
                vAlign = 0.5f;
            else if (_verticalAlign == VerticalAlign.Bottom)
                vAlign = 1f;
            
            // As the layoutElements are positioned, we'll count how many rows 
            // fall within the layoutTarget's scrollRect
            int visibleColumns = 0;
            float minVisibleX = layoutTarget.HorizontalScrollPosition;
            float maxVisibleX = minVisibleX + targetWidth;
            
            // Finally, position the layoutElements and find the first/last
            // visible indices, the content size, and the number of 
            // visible elements.    
            float x = _paddingLeft;
            float y0 = _paddingTop;
            float maxX = _paddingLeft;
            float maxY = _paddingTop;
            int firstColInView = -1;
            int lastColInView = -1;

            // Take horizontalAlign into account
            if (excessWidth > 0 || !ClipAndEnableScrolling)
            {
                HorizontalAlign hAlign = _horizontalAlign;
                if (hAlign == HorizontalAlign.Center)
                {
                    x = _paddingLeft + Mathf.Round(excessWidth / 2);   
                }
                else if (hAlign == HorizontalAlign.Right)
                {
                    x = _paddingLeft + excessWidth;   
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

                float y = y0 + (containerHeight - dy) * vAlign;
                // In case we have HorizontalAlign.CENTER we have to round
                if (vAlign == 0.5)
                    y = Mathf.Round(y);
                LayoutUtil.SetLayoutBoundsPosition((InvalidationManagerClient) layoutElement, x, y);
                                
                // Update maxX,Y, first,lastVisibleIndex, and y
                maxX = Mathf.Max(maxX, x + dx);
                maxY = Mathf.Max(maxY, y + dy);
                if (!ClipAndEnableScrolling ||
                    ((x < maxVisibleX) && ((x + dx) > minVisibleX)) ||
                    ((dx <= 0) && ((x == maxVisibleX) || (x == minVisibleX))))
                {
                    visibleColumns += 1;
                    if (firstColInView == -1)
                       firstColInView = lastColInView = index;
                    else
                       lastColInView = index;
                }
                x += dx + _gap;
            }

            SetColumnCount(visibleColumns);
            SetIndexInView(firstColInView, lastColInView);
            
            // Make sure that if the content spans partially over a pixel to the right/bottom,
            // the content size includes the whole pixel.
            layoutTarget.SetContentSize(Mathf.Ceil(maxX + _paddingRight),
                                        Mathf.Ceil(maxY + _paddingBottom));
        }

        /**
         *  Returns: Returns the maximum value for an element's dimension so that the component doesn't
         *  spill out of the container size. Calculations are based on the layout rules.
         *  Pass in unscaledWidth, hCenter, left, right, childX to get a maxWidth value.
         *  Pass in unscaledHeight, vCenter, top, bottom, childY to get a maxHeight value.
         */
        /*static private float MaxSizeToFitIn(float totalSize,
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
        }*/

        /*private static bool ConstraintsDetermineWidth(ILayoutElement layoutElement)
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
        public int FirstIndexInView
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
        public int LastIndexInView
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
                if (navigationUnit == NavigationUnit.Left)
                    return arrowKeysWrapFocus ? maxIndex : -1;

                if (navigationUnit == NavigationUnit.Right)
                    return 0;
            }

            currentIndex = Math.Max(0, Math.Min(maxIndex, currentIndex));

            int newIndex;
            Rectangle bounds;
            float x;

            switch (navigationUnit)
            {
                case NavigationUnit.Left:
                    {
                        if (arrowKeysWrapFocus && currentIndex == 0)
                            newIndex = maxIndex;
                        else
                            newIndex = currentIndex - 1;
                        break;
                    }

                case NavigationUnit.Right:
                    {
                        if (arrowKeysWrapFocus && currentIndex == maxIndex)
                            newIndex = 0;
                        else
                            newIndex = currentIndex + 1;
                        ;
                        break;
                    }

                case NavigationUnit.PageUp:
                case NavigationUnit.PageLeft:
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
                            x = GetVerticalScrollPositionDelta(NavigationUnit.PageLeft) + GetScrollRect().Left;
                        }
                        else
                        {
                            // currentIndex is not visible, just find an element a page up from currentIndex
                            x = GetElementBounds(currentIndex).Right - GetScrollRect().Width;
                        }

                        // Find the element after the last element that spans above the y position
                        newIndex = currentIndex - 1;
                        while (0 <= newIndex)
                        {
                            bounds = GetElementBounds(newIndex);
                            if (null != bounds && bounds.Left < x)
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
                case NavigationUnit.PageRight:
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
                            x = GetVerticalScrollPositionDelta(NavigationUnit.PageRight) + GetScrollRect().Right;
                        }
                        else
                        {
                            // currentIndex is not visible, just find an element a page down from currentIndex
                            x = GetElementBounds(currentIndex).Left + GetScrollRect().Width;
                        }

                        // Find the element before the first element that spans below the y position
                        newIndex = currentIndex + 1;
                        while (newIndex <= maxIndex)
                        {
                            bounds = GetElementBounds(newIndex);
                            if (null != bounds && bounds.Right > x)
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
            float eltX;
            float eltWidth;
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
            eltX = LayoutUtil.GetLayoutBoundsX((InvalidationManagerClient)elt); // elt.getLayoutBoundsY();
            eltWidth = LayoutUtil.GetLayoutBoundsWidth((InvalidationManagerClient)elt); // elt.getLayoutBoundsHeight();
            //}

            // So, index is either the first or last row in the scrollRect
            // and potentially partially visible.
            //   y0,y1 - scrollRect top,bottom edges
            //   iy0, iy1 - layout element top,bottom edges
            var x0 = g.HorizontalScrollPosition;
            var x1 = x0 + g.Width;
            var ix0 = eltX;
            var ix1 = ix0 + eltWidth;
            if (ix0 >= ix1)  // element has 0 or negative width
                return 1.0f;
            if ((ix0 >= x0) && (ix1 <= x1))
                return 1.0f;
            return (Math.Min(x1, ix1) - Math.Max(x0, ix0)) / (ix1 - ix0);
        }

        // TEMP commented
        /*internal override Rectangle GetElementBoundsLeftOfScrollRect(Rectangle scrollRect)
        {
            return FindLayoutElementBounds(Target, FirstIndexInView, -1, scrollRect);
        }*/

        // TEMP commented
        /*internal override Rectangle GetElementBoundsRightOfScrollRect(Rectangle scrollRect)
        {
            return FindLayoutElementBounds(Target, LastIndexInView, 1, scrollRect);
        }*/

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
                    return new Rectangle(0, 0, PaddingLeft, 0);
                if (i >= n)
                    return new Rectangle(GetElementBounds(n-1).Right, 0, PaddingRight, 0);
            }

            while((i >= 0) && (i < n))
            {
               var elementR = GetElementBounds(i);
               // Special case: if the scrollRect r _only_ contains
               // elementR, then if we're searching left (dir == -1),
               // and elementR's left edge is visible, then try again
               // with i-1.   Likewise for dir == +1.
               if (null != elementR)
               {
                   var overlapsLeft = (dir == -1) && (elementR.Left == r.Left) && (elementR.Right >= r.Right);
                   var overlapsRight = (dir == +1) && (elementR.Right == r.Right) && (elementR.Left <= r.Left);
                   if (!(overlapsLeft || overlapsRight))             
                       return elementR;               
               }
               i += dir;
            }
            return null;
        }*/

        /**
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

            var x0 = scrollR.Left;
            var x1 = scrollR.Right - 0.0001f;
            if (x1 <= x0)
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
            i0 = FindIndexAt(x0 + Gap, Gap, g, 0, n);
            i1 = FindIndexAt(x1, Gap, g, 0, n);
            /*}*/

            // Special case: no element overlaps y0, is index 0 visible?
            if (i0 == -1)
            {
                var index0 = FindLayoutElementIndex(g, 0, 1);
                if (index0 != -1)
                {
                    var element0 = (InvalidationManagerClient)g.GetContentChildAt(index0);
                    var element0X = LayoutUtil.GetLayoutBoundsX(element0);
                    var element0Width = LayoutUtil.GetLayoutBoundsWidth(element0);
                    if ((element0X < x1) && ((element0X + element0Width) > x0))
                        i0 = index0;
                }
            }

            // Special case: no element overlaps y1, is index 0 visible?
            if (i1 == -1)
            {
                var index1 = FindLayoutElementIndex(g, n, -1);
                if (index1 != -1)
                {
                    var element1 = (InvalidationManagerClient)g.GetContentChildAt(index1);
                    var element1X = LayoutUtil.GetLayoutBoundsX(element1);
                    var element1width = LayoutUtil.GetLayoutBoundsWidth(element1);
                    if ((element1X < x1) && ((element1X + element1width) > x0))
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
            InvalidationManagerClient element = (InvalidationManagerClient)g.GetContentChildAt(index);
            var elementY = LayoutUtil.GetLayoutBoundsY(element);
            var elementHeight = LayoutUtil.GetLayoutBoundsHeight(element);
            // TBD: deal with null element, includeInLayout false.
            if ((y >= elementY) && (y < elementY + elementHeight + gap))
                return index;
            else if (i0 == i1)
                return -1;
            else if (y < elementY)
                return FindIndexAt(y, gap, g, i0, Math.Max(i0, index - 1));
            else
                return FindIndexAt(y, gap, g, Math.Min(index + 1, i1), i1);
        }

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
            while ((i >= 0) && (i < n))
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
    }
}