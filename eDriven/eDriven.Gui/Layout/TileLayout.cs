using System;
using eDriven.Core.Geom;
using eDriven.Gui.Components;
using eDriven.Gui.Events;
using UnityEngine;

namespace eDriven.Gui.Layout
{
    public sealed class TileLayout : LayoutBase
    {
        /**
         *  
         *  storage for old property values, in order to dispatch change events.
         */
        private float? _oldColumnWidth;
        private float? _oldRowHeight;
        private int _oldColumnCount = -1;
        private int _oldRowCount = -1;
        private float? _oldHorizontalGap;
        private float? _oldVerticalGap;

        // Cache storage to avoid repeating work from measure() in updateDisplayList().
        // These are set the first time the value is calculated and are reset at the end
        // of updateDisplayList().
        private float? _tileWidthCached;
        private float? _tileHeightCached;
        private int _numElementsCached = -1;
        
        /**
         *  
         *  The following variables are used by updateDisplayList() and set by
         *  calculateDisplayParameters().   If virtualLayout=true they're based 
         *  on the current scrollRect.
         */
        private int _visibleStartIndex = -1;   // dataProvider/layout element index  
        private int _visibleEndIndex = -1;     // ...
        private float _visibleStartX;     // first tile/cell origin
        private float _visibleStartY;     // ...

        //----------------------------------
        //  horizontalGap
        //----------------------------------

        private float _explicitHorizontalGap = 6;
        private float _horizontalGap = 6;

        /**
         *  Horizontal space between columns, in pixels.
         *  Default: 6
         */
        public float HorizontalGap
        {
            get
            {
                return _horizontalGap;
            }
            set
            {
                _explicitHorizontalGap = value;
                if (value == _horizontalGap)
                    return;

                _horizontalGap = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  verticalGap
        //----------------------------------

        private float _explicitVerticalGap = 6;
        private float _verticalGap = 6;

        /**
         *  Vertical space between rows, in pixels.
         *  Default: 6
         */
        public float VerticalGap
        {
            get
            {
                return _verticalGap;
            }
            set
            {
                _explicitVerticalGap = value;
                if (value == _verticalGap)
                    return;

                _verticalGap = value;
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
                _columnCount = value;
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
        //  columnWidth
        //----------------------------------

        private float? explicitColumnWidth;
        private float? _columnWidth;

        /**
         *  Contain the actual column width, in pixels.
         *
         *  <p>If not explicitly set, the column width is 
         *  determined from the width of the widest element. </p>
         *
         *  <p>If the <code>columnAlign</code> property is set 
         *  to <code>"justifyUsingWidth"</code>,  the column width grows to the 
         *  container width to justify the fully-visible columns.</p>
         */
        public float? ColumnWidth
        {
            get
            {
                return _columnWidth;
            }
            set
            {
                explicitColumnWidth = value;
            if (value == _columnWidth)
                return;

            _columnWidth = value;
            InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  rowHeight
        //----------------------------------

        private float? explicitRowHeight;
        private float? _rowHeight;

        /**
         *  The row height, in pixels.
         *
         *  <p>If not explicitly set, the row height is 
         *  determined from the maximum of elements' height.</p>
         *
         *  If <code>rowAlign</code> is set to "justifyUsingHeight", the actual row height
         *  increases to justify the fully-visible rows to the container height.
         */
        public float? RowHeight
        {
            get
            {
                return _rowHeight;
            }
            set
            {
                explicitRowHeight = value;
                if (value == _rowHeight)
                    return;

                _rowHeight = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        /**
         *  
         */
        private HorizontalAlign _horizontalAlign = HorizontalAlign.Justify;

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
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  verticalAlign
        //----------------------------------

        /**
         *  
         */
        private VerticalAlign _verticalAlign = VerticalAlign.Justify;

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
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  columnAlign
        //----------------------------------

        private ColumnAlign _columnAlign = ColumnAlign.Left;

        /**
         *  Specifies how to justify the fully visible columns to the container width.
         *  ActionScript values can be <code>ColumnAlign.LEFT</code>, <code>ColumnAlign.JUSTIFY_USING_GAP</code>
         *  and <code>ColumnAlign.JUSTIFY_USING_WIDTH</code>.
         *  MXML values can be <code>"left"</code>, <code>"justifyUsingGap"</code> and <code>"justifyUsingWidth"</code>.
         *
         *  <p>When set to <code>ColumnAlign.LEFT</code> it turns column justification off.
         *  There may be partially visible columns or whitespace between the last column and
         *  the right edge of the container.  This is the default value.</p>
         *
         *  <p>When set to <code>ColumnAlign.JUSTIFY_USING_GAP</code> the <code>horizontalGap</code>
         *  actual value increases so that
         *  the last fully visible column right edge aligns with the container's right edge.
         *  In case there is only a single fully visible column, the <code>horizontalGap</code> actual value
         *  increases so that it pushes any partially visible column beyond the right edge
         *  of the container.  
         *  Note that explicitly setting the <code>horizontalGap</code> property does not turn off
         *  justification. It only determines the initial gap value. 
         *  Justification may increases it.</p>
         *
         *  <p>When set to <code>ColumnAlign.JUSTIFY_USING_WIDTH</code> the <code>columnWidth</code>
         *  actual value increases so that
         *  the last fully visible column right edge aligns with the container's right edge.  
         *  Note that explicitly setting the <code>columnWidth</code> property does not turn off justification.
         *  It only determines the initial column width value. 
         *  Justification may increases it.</p>
         */
        public ColumnAlign ColumnAlign
        {
            get
            {
                return _columnAlign;
            }
            set
            {
                if (_columnAlign == value)
                    return;

                _columnAlign = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  rowAlign
        //----------------------------------

        private RowAlign _rowAlign = RowAlign.Top;

        /**
         *  Specifies how to justify the fully visible rows to the container height.
         *  ActionScript values can be <code>RowAlign.TOP</code>, <code>RowAlign.JUSTIFY_USING_GAP</code>
         *  and <code>RowAlign.JUSTIFY_USING_HEIGHT</code>.
         *  MXML values can be <code>"top"</code>, <code>"justifyUsingGap"</code> and <code>"justifyUsingHeight"</code>.
         *
         *  <p>When set to <code>RowAlign.TOP</code> it turns column justification off. 
         *  There might be partially visible rows or whitespace between the last row and
         *  the bottom edge of the container.  This is the default value.</p>
         *
         *  <p>When set to <code>RowAlign.JUSTIFY_USING_GAP</code> the <code>verticalGap</code>
         *  actual value increases so that
         *  the last fully visible row bottom edge aligns with the container's bottom edge.
         *  In case there is only a single fully visible row, the value of <code>verticalGap</code> 
         *  increases so that it pushes any partially visible row beyond the bottom edge
         *  of the container.  Note that explicitly setting the <code>verticalGap</code> does not turn off
         *  justification, but just determines the initial gap value.
         *  Justification can then increases it.</p>
         *
         *  <p>When set to <code>RowAlign.JUSTIFY_USING_HEIGHT</code> the <code>rowHeight</code>
         *  actual value increases so that
         *  the last fully visible row bottom edge aligns with the container's bottom edge.  Note that
         *  explicitly setting the <code>rowHeight</code> does not turn off justification, but 
         *  determines the initial row height value.
         *  Justification can then increases it.</p>
         */
        public RowAlign RowAlign
        {
            get
            {
                return _rowAlign;
            }
            set
            {
                if (_rowAlign == value)
                    return;

                _rowAlign = value;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        //----------------------------------
        //  orientation
        //----------------------------------

        private TileOrientation _orientation = TileOrientation.Rows;

        /**
         *  Specifies whether elements are arranged row by row or
         *  column by column.
         *  ActionScript values can be <code>TileOrientation.ROWS</code> and 
         *  <code>TileOrientation.COLUMNS</code>.
         *  MXML values can be <code>"rows"</code> and <code>"columns"</code>.
         *
         *  Default: TileOrientation.ROWS
         */
        public TileOrientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (_orientation == value)
                    return;

                _orientation = value;
                _tileWidthCached = _tileHeightCached = null;
                InvalidateTargetSizeAndDisplayList();
            }
        }

        ///**
        // *  
        // */
        //public override bool UseVirtualLayout
        //{
        //    get
        //    {
        //        return base.UseVirtualLayout;
        //    }
        //    set
        //    {
        //        if (base.UseVirtualLayout == value)
        //            return;

        //        base.UseVirtualLayout = value;

        //        // Reset the state that virtual depends on.  If the layout has already
        //        // run with useVirtualLayout=false, the visibleStartEndIndex variables
        //        // will have been set to 0, dataProvider.length.
        //        if (value)
        //        {
        //            _visibleStartIndex = -1;
        //            _visibleEndIndex = -1;
        //            _visibleStartX = 0;
        //            _visibleStartY = 0;
        //        }
        //    }
        //}

        internal override void ClearVirtualLayoutCache()
        {
            _tileWidthCached = _tileHeightCached = null;
        }

        /**
         *  Dispatches events if Actual values have changed since the last call.
         *  Checks columnWidth, rowHeight, columnCount, rowCount, horizontalGap, verticalGap.
         *  This method is called from within updateDisplayList()
         */
        private void DispatchEventsForActualValueChanges()
        {
            if (HasEventListener(PropertyChangeEvent.PROPERTY_CHANGE))
            {
                if (_oldColumnWidth != _columnWidth)
                    DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "columnWidth", _oldColumnWidth, _columnWidth));
                if (_oldRowHeight != _rowHeight)
                    DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "rowHeight", _oldRowHeight, _rowHeight));
                if (_oldColumnCount != _columnCount)
                    DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "columnCount", _oldColumnCount, _columnCount));
                if (_oldRowCount != _rowCount)
                    DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "rowCount", _oldRowCount, _rowCount));
                if (_oldHorizontalGap != _horizontalGap)
                    DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "horizontalGap", _oldHorizontalGap, _horizontalGap));
                if (_oldVerticalGap != _verticalGap)
                    DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "verticalGap", _oldVerticalGap, _verticalGap));
            }

            _oldColumnWidth = _columnWidth;
            _oldRowHeight = _rowHeight;
            _oldColumnCount = _columnCount;
            _oldRowCount = _rowCount;
            _oldHorizontalGap = _horizontalGap;
            _oldVerticalGap = _verticalGap;
        }

        /**
         *  This method is called from measure() and updateDisplayList() to calculate the
         *  actual values for columnWidth, rowHeight, columnCount, rowCount, horizontalGap and verticalGap.
         *
         *  Param: width - the width during measure() is the layout target explicitWidth or NaN
         *  and during updateDisplayList() is the unscaledWidth.
         *  Param: height - the height during measure() is the layout target explicitHeight or NaN
         *  and during updateDisplayList() is the unscaledHeight.
         */
        private void UpdateActualValues(float width, float height)
        {
            // First, figure the tile size
            calculateTileSize();

            // Second, figure out number of rows/columns
            int elementCount = calculateElementCount();
            calculateColumnAndRowCount(width, height, elementCount);

            // Third, adjust the gaps and column and row sizes based on justification settings
            _horizontalGap = _explicitHorizontalGap;
            _verticalGap = _explicitVerticalGap;

            // Justify
            switch (_columnAlign)
            {
                case ColumnAlign.JustifyUsingGap:
                    _horizontalGap = justifyByGapSize(width, (float) _columnWidth, _horizontalGap, _columnCount);
                break;
                case ColumnAlign.JustifyUsingWidth:
                    _columnWidth = justifyByElementSize(width, (float) _columnWidth, _horizontalGap, _columnCount);
                break;
            }

            switch (_rowAlign)
            {
                case RowAlign.JustifyUsingGap:
                    _verticalGap = justifyByGapSize(height, (float) _rowHeight, _verticalGap, _rowCount);
                break;
                case RowAlign.JustifyUsingHeight:
                    _rowHeight = justifyByElementSize(height, (float) _rowHeight, _verticalGap, _rowCount);
                break;
            }

            // Last, if we have explicit overrides for both rowCount and columnCount, then
            // make sure that column/row count along the minor axis reflects the actual count.
            if (-1 != _requestedColumnCount && -1 != _requestedRowCount)
            {
                if (_orientation == TileOrientation.Rows)
// ReSharper disable PossibleLossOfFraction
                    _rowCount = (int) Math.Max(1, Mathf.Ceil(elementCount / Math.Max(1, _requestedColumnCount)));
                else
                    _columnCount = (int) Math.Max(1, Mathf.Ceil(elementCount / Math.Max(1, _requestedRowCount)));
// ReSharper restore PossibleLossOfFraction
            }
        }

        /**
         *  
         *  Returns true, if the dimensions (colCount1, rowCount1) are more square than (colCount2, rowCount2).
         *  Squareness is the difference between width and height of a tile layout
         *  with the specified number of columns and rows.
         */
        private bool closerToSquare(int colCount1, int rowCount1, int colCount2, int rowCount2)
        {
            float difference1 = Math.Abs((float) (colCount1 * (_columnWidth + _horizontalGap) - _horizontalGap - 
                                                  rowCount1 * (_rowHeight   +   _verticalGap) + _verticalGap));
            float difference2 = Math.Abs((float) (colCount2 * (_columnWidth + _horizontalGap) - _horizontalGap - 
                                                  rowCount2 * (_rowHeight   +   _verticalGap) + _verticalGap));
            
            return difference1 < difference2 || (difference1 == difference2 && rowCount1 <= rowCount2);
        }

        /**
         *  
         *  Calculates _columnCount and _rowCount based on width, height,
         *  orientation, _requestedColumnCount, _requestedRowCount, _columnWidth, _rowHeight.
         *  _columnWidth and _rowHeight must be valid before calling.
         */
        private void calculateColumnAndRowCount(float? width, float? height, int elementCount)
        {
            _columnCount = _rowCount = -1;

            if (-1 != _requestedColumnCount || -1 != _requestedRowCount)
            {
                if (-1 != _requestedRowCount)
                    _rowCount = Math.Max(1, _requestedRowCount);

                if (-1 != _requestedColumnCount)
                    _columnCount = Math.Max(1, _requestedColumnCount);
            }
            // Figure out number of columns or rows based on the explicit size along one of the axes
            else if (null != width && (_orientation == TileOrientation.Rows || null == height))
            {
                if (_columnWidth + _explicitHorizontalGap > 0)
                    _columnCount = Math.Max(1, (int)Mathf.Floor((float) ((width + _explicitHorizontalGap) / (_columnWidth + _explicitHorizontalGap))));
                else
                    _columnCount = 1;
            }
            else if (null != height && (_orientation == TileOrientation.Columns || null == width))
            {
                if (_rowHeight + _explicitVerticalGap > 0)
                    _rowCount = Math.Max(1, (int)Mathf.Floor((float) ((height + _explicitVerticalGap) / (_rowHeight + _explicitVerticalGap))));
                else
                    _rowCount = 1;
            }
            else // Figure out the number of columns and rows so that pixels area occupied is as square as possible
            {
                // Calculate number of rows and columns so that
                // pixel area is as square as possible
                float hGap = _explicitHorizontalGap;
                float vGap = _explicitVerticalGap;

                // 1. columnCount * (columnWidth + hGap) - hGap == rowCount * (rowHeight + vGap) - vGap
                // 1. columnCount * (columnWidth + hGap) == rowCount * (rowHeight + vGap) + hGap - vGap
                // 1. columnCount == (rowCount * (rowHeight + vGap) + hGap - vGap) / (columnWidth + hGap)
                // 2. columnCount * rowCount == elementCount
                // substitute 1. in 2.
                // rowCount * rowCount + (hGap - vGap) * rowCount - elementCount * (columnWidth + hGap ) == 0

                float a = Math.Max(0, ((float)_rowHeight + vGap));
                float b = (hGap - vGap);
                float c = -elementCount * ((float)_columnWidth + hGap);
                float d = b * b - 4 * a * c; // Always guaranteed to be greater than zero, since c <= 0
                d = Mathf.Sqrt(d);

                // We are guaranteed that we have only one positive root, since d >= b:
                float rowCount = (a != 0) ? (b + d) / (2 * a) : elementCount;

// ReSharper disable PossibleLossOfFraction

                // To get integer count for the columns/rows we round up and down so
                // we get four possible solutions. Then we pick the best one.
                int row1 = (int) Math.Max(1, Math.Floor(rowCount));
                int col1 = (int) Math.Max(1, Mathf.Ceil(elementCount / row1));
                row1 = (int) Math.Max(1, Mathf.Ceil(elementCount / col1));

                int row2 = (int) Math.Max(1, Mathf.Ceil(rowCount));
                int col2 = (int) Math.Max(1, Mathf.Ceil(elementCount / row2));
                row2 = (int) Math.Max(1, Mathf.Ceil(elementCount / col2));

                int col3 = (int) Math.Max(1, Mathf.Floor(elementCount / rowCount));
                int row3 = (int) Math.Max(1, Mathf.Ceil(elementCount / col3));
                col3 = (int) Math.Max(1, Mathf.Ceil(elementCount / row3));

                int col4 = (int) Math.Max(1, Mathf.Ceil(elementCount / rowCount));
                int row4 = (int) Math.Max(1, Mathf.Ceil(elementCount / col4));
                col4 = (int) Math.Max(1, Mathf.Ceil(elementCount / row4));

// ReSharper restore PossibleLossOfFraction

                if (closerToSquare(col3, row3, col1, row1))
                {
                    col1 = col3;
                    row1 = row3;
                }

                if (closerToSquare(col4, row4, col2, row2))
                {
                    col2 = col4;
                    row2 = row4;
                }

                if (closerToSquare(col1, row1, col2, row2))
                {
                    _columnCount = col1;
                    _rowCount = row1;
                }
                else
                {
                    _columnCount = col2;
                    _rowCount = row2;
                }
            }

// ReSharper disable PossibleLossOfFraction

            // In case we determined only columns or rows (from explicit overrides or explicit width/height)
            // calculate the other from the number of elements
            if (-1 == _rowCount)
                _rowCount = (int) Math.Max(1, Mathf.Ceil(elementCount / _columnCount));
            if (-1 == _columnCount)
                _columnCount = (int) Math.Max(1, Mathf.Ceil(elementCount / _rowCount));

// ReSharper restore PossibleLossOfFraction
        }

        /**
         *  
         *  Increases the gap so that elements are justified to exactly fit totalSize
         *  leaving no partially visible elements in view.
         *  Returns: Returs the new gap size.
         */
        private static float justifyByGapSize(float totalSize, float elementSize,
                                          float gap, int elementCount)
        {
            // If element + gap collapses to zero, then don't adjust the gap.
            if (elementSize + gap <= 0)
                return gap;

            // Find the number of fully visible elements
            int visibleCount =
                (int) Math.Min(elementCount, Math.Floor((totalSize + gap) / (elementSize + gap)));

            // If there isn't even a singel fully visible element, don't adjust the gap
            if (visibleCount < 1)
                return gap;

            // Special case: if there's a singe fully visible element and a partially
            // visible element, then make the gap big enough to push out the partially
            // visible element out of view.
            if (visibleCount == 1)
                return elementCount > 1 ? Math.Max(gap, totalSize - elementSize) : gap;

            // Now calculate the gap such that the fully visible elements and gaps
            // add up exactly to totalSize:
            // <==> totalSize == visibleCount * elementSize + (visibleCount - 1) * gap
            // <==> totalSize - visibleCount * elementSize == (visibleCount - 1) * gap
            // <==> (totalSize - visibleCount * elementSize) / (visibleCount - 1) == gap
            return (totalSize - visibleCount * elementSize) / (visibleCount - 1);
        }

        /**
         *  
         *  Increases the element size so that elements are justified to exactly fit
         *  totalSize leaving no partially visible elements in view.
         *  Returns: Returns the the new element size.
         */
        private float justifyByElementSize(float totalSize, float elementSize,
                                              float gap, int elementCount)
        {
            float elementAndGapSize = elementSize + gap;
            int visibleCount = 0;
            // Find the number of fully visible elements
            if (elementAndGapSize == 0)
                visibleCount = elementCount;
            else
                visibleCount = (int) Math.Min(elementCount, Math.Floor((totalSize + gap) / elementAndGapSize));

            // If there isn't event a single fully visible element, don't adjust
            if (visibleCount < 1)
                return elementSize;

            // Now calculate the elementSize such that the fully visible elements and gaps
            // add up exactly to totalSize:
            // <==> totalSize == visibleCount * elementSize + (visibleCount - 1) * gap
            // <==> totalSize - (visibleCount - 1) * gap == visibleCount * elementSize
            // <==> (totalSize - (visibleCount - 1) * gap) / visibleCount == elementSize
            return (totalSize - (visibleCount - 1) * gap) / visibleCount;
        }

        /**
         *  
         *  Update _tileWidth,Height to be the maximum of their current
         *  value and the element's preferred bounds. 
         */
        private void updateVirtualTileSize(ILayoutElement elt)
        {
            if (null == elt || !elt.IncludeInLayout)
                return;
            float w = LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) elt);
            float h = LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) elt);
            _tileWidthCached = null == _tileWidthCached ? w : Mathf.Max(w, (float)_tileWidthCached);
            _tileHeightCached = null == _tileHeightCached ? h : Mathf.Max(h, (float)_tileHeightCached);
        }

        /**
         *  
         */
        private void calculateVirtualTileSize()
        {
            // If both dimensions are explicitly set, we're done
            _columnWidth = explicitColumnWidth;
            _rowHeight = explicitRowHeight;
            if (null != _columnWidth && null != _rowHeight)
            {
                _tileWidthCached = _columnWidth;
                _tileHeightCached = _rowHeight;
                return;
            }
            
            // update _tileWidth,HeightCached based on the typicalElement     
            updateVirtualTileSize(TypicalLayoutElement);
            
            //// update _tileWidth,HeightCached based on visible elements    
            //if ((visibleStartIndex != -1) && (visibleEndIndex != -1))
            //{
            //    for (var index:int = visibleStartIndex; index <= visibleEndIndex; index++)
            //        updateVirtualTileSize(target.getVirtualElementAt(index));
            //}
            
            // Make sure that we always have non-NaN values in the cache, even
            // when there are no elements.
            if (null == _tileWidthCached)
                _tileWidthCached = 0;
            if (null == _tileHeightCached)
                _tileHeightCached = 0;
            
            if (null == _columnWidth)
                _columnWidth = _tileWidthCached;
            if (null == _rowHeight)
                _rowHeight = _tileHeightCached;        
        }

        /**
         *  
         *  Calculates _columnWidth and _rowHeight from maximum of
         *  elements preferred size and any explicit overrides.
         */
        private void calculateRealTileSize()
        {
            _columnWidth = _tileWidthCached;
            _rowHeight = _tileHeightCached;
            if (null != _columnWidth && null != _rowHeight)
                return;

            // Are both dimensions explicitly set?
            _columnWidth = _tileWidthCached = explicitColumnWidth;
            _rowHeight = _tileHeightCached = explicitRowHeight;
            if (null != _columnWidth && null != _rowHeight)
                return;

            // Find the maxmimums of element's preferred sizes
            float columnWidth = 0;
            float rowHeight = 0;

            GroupBase layoutTarget = Target;
            int count = layoutTarget.NumberOfChildren;
            // Remember the number of includeInLayout elements
            _numElementsCached = count;
            for (int i = 0; i < count; i++)
            {
                ILayoutElement el = (ILayoutElement) layoutTarget.GetChildAt(i);
                if (null == el || !el.IncludeInLayout)
                {
                    _numElementsCached--;
                    continue;
                }

                if (null == _columnWidth)
                    columnWidth = Math.Max(columnWidth, LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) el));
                if (null == _rowHeight)
                    rowHeight = Math.Max(rowHeight, LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient)el));
            }

            if (null == _columnWidth)
                _columnWidth = _tileWidthCached = columnWidth;
            if (null == _rowHeight)
                _rowHeight = _tileHeightCached = rowHeight;
        }

        private void calculateTileSize()
        {
            /*if (UseVirtualLayout)
                calculateVirtualTileSize();
            else */
                calculateRealTileSize();
        }

        /**
         *  
         *  For normal layout return the number of non-null includeInLayout=true
         *  layout elements, for virtual layout just return the number of layout
         *  elements.
         */
        private int calculateElementCount()
        {
            if (-1 != _numElementsCached)
                return _numElementsCached;
                
            GroupBase layoutTarget = Target;
            int count = layoutTarget.NumberOfChildren;
            _numElementsCached = count;

            /*if (UseVirtualLayout)
                return _numElementsCached;*/
                
            for (int i = 0; i < count; i++)
            {
                ILayoutElement el = (ILayoutElement) layoutTarget.GetChildAt(i);
                if (null == el || !el.IncludeInLayout)
                    _numElementsCached--;
            }

            return _numElementsCached;
        }

        /**
         *  
         *  This method computes values for visibleStartX,Y, visibleStartIndex, and 
         *  visibleEndIndex based on the TileLayout geometry values, like _columnWidth
         *  and _rowHeight, computed by calculateActualValues().
         * 
         *  If useVirtualLayout=false, then visibleStartX,Y=0 and visibleStartIndex=0
         *  and visibleEndIndex=layoutTarget.numElements-1.
         * 
         *  If useVirtualLayout=true and orientation=ROWS then visibleStartIndex is the 
         *  layout element index of the item at first visible row relative to the scrollRect, 
         *  column 0.  Note that we're using column=0 instead of the first visible column
         *  to simplify the iteration logic in updateDisplayList().  This is optimal 
         *  for the common case where the entire row is visible.   Optimally handling 
         *  the case where orientation=ROWS and each row is only partially visible is 
         *  doable but adds some complexity to the main loop.
         * 
         *  The logic for useVirtualLayout=true and orientation=COLS is similar.
         */
        private void calculateDisplayParameters(int unscaledWidth, int unscaledHeight)
        {
            UpdateActualValues(unscaledWidth, unscaledHeight);

            GroupBase layoutTarget = Target;
            int eltCount = layoutTarget.NumberOfChildren;
            _visibleStartX = 0;   // initial values for xPos,yPos in updateDisplayList
            _visibleStartY = 0;
            _visibleStartIndex = 0;
            _visibleEndIndex = eltCount - 1;

            /*if (UseVirtualLayout)
            {
                float hsp = layoutTarget.HorizontalScrollPosition;
                float vsp = layoutTarget.VerticalScrollPosition;
                float cwg = (float) (_columnWidth + _horizontalGap);
                float rwg = (float) (_rowHeight + _verticalGap);
                
                int visibleCol0 = (int) Math.Max(0, Mathf.Floor(hsp / cwg));
                int visibleRow0 = (int) Math.Max(0, Mathf.Floor(vsp / rwg));
                int visibleCol1 = (int) Math.Max(_columnCount - 1, Mathf.Floor((hsp + unscaledWidth) / cwg));
                int visibleRow1 = (int) Math.Max(_rowCount - 1, Mathf.Floor((vsp + unscaledHeight) / rwg));

                if (_orientation == TileOrientation.Rows)
                {
                    _visibleStartIndex = (visibleRow0 * _columnCount);
                    _visibleEndIndex = Math.Min(eltCount - 1, (visibleRow1 * _columnCount) + visibleCol1);
                    _visibleStartY = visibleRow0 * rwg;
                }
                else
                {
                    _visibleStartIndex = (visibleCol0 * _rowCount);
                    _visibleEndIndex = Math.Min(eltCount - 1, (visibleCol1 * _rowCount) + visibleRow1);
                    _visibleStartX = visibleCol0 * cwg;                
                }
            }*/
        }

        /**
         *  
         *  This method is called by updateDisplayList() after initial values for 
         *  visibleStartIndex, visibleEndIndex have been calculated.  We 
         *  re-calculateDisplayParameters() to account for the possibility that
         *  larger cells may have been exposed.  Since tileWdth,Height can only
         *  increase, the new visibleStart,EndIndex values will be greater than or
         *  equal to the old ones. 
         */
         /*private void updateVirtualLayout(int unscaledWidth, int unscaledHeight)
         {
            int oldVisibleStartIndex = _visibleStartIndex;
            int oldVisibleEndIndex = _visibleEndIndex;
            calculateDisplayParameters(unscaledWidth, unscaledHeight);  // compute new visibleStart,EndIndex values
            
            // We're responsible for laying out *all* of the elements requested
            // with getVirtualElementAt(), even if they don't fall within the final
            // visible range.  Hide any extra ones.  On the next layout pass, they'll
            // be added to DataGroup::freeRenderers
            
            GroupBase layoutTarget = Target;
            for (int i = oldVisibleStartIndex; i <= oldVisibleEndIndex; i++)
            {
                if ((i >= _visibleStartIndex) && (i <= _visibleEndIndex)) // skip past the visible range
                {
                    i = _visibleEndIndex;
                    continue;
                } 
                //ILayoutElement el = layoutTarget.getVirtualElementAt(i);
                //if (null == el)
                //    continue;
                //el.setLayoutBoundsSize(0, 0);
                //if (el is IVisualElement)
                //    IVisualElement(el).visible = false; 
            }
        } */

        /**
         *  Sets the size and the position of the specified layout element and cell bounds.
         *  Param: element - the element to resize and position.
         *  Param: cellX - the x coordinate of the cell.
         *  Param: cellY - the y coordinate of the cell.
         *  Param: cellWidth - the width of the cell.
         *  Param: cellHeight - the height of the cell.
         */
        private void sizeAndPositionElement(ILayoutElement element,
                                                  int cellX,
                                                  int cellY,
                                                  int cellWidth,
                                                  int cellHeight)
        {
            float childWidth;
            float childHeight;

            // Determine size of the element
            if (_horizontalAlign == HorizontalAlign.Justify)
                childWidth = cellWidth;
            else if (null != element.PercentWidth)
                childWidth = Mathf.Round((float) (cellWidth * element.PercentWidth * 0.01f));
            else
                childWidth = LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) element);

            if (_verticalAlign == global::eDriven.Gui.Layout.VerticalAlign.Justify)
                childHeight = cellHeight;
            else if (null != element.PercentHeight)
                childHeight = Mathf.Round((float) (cellHeight * element.PercentHeight * 0.01f));
            else
                childHeight = LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) element);

            // Enforce min and max limits
            float maxChildWidth = Math.Min(LayoutUtil.GetMaxBoundsWidth((InvalidationManagerClient) element), cellWidth);
            float maxChildHeight = Math.Min(LayoutUtil.GetMaxBoundsHeight((InvalidationManagerClient) element), cellHeight);
            // Make sure we enforce element's minimum last, since it has the highest priority
            childWidth = Math.Max(LayoutUtil.GetMinBoundsWidth((InvalidationManagerClient) element), Math.Min(maxChildWidth, childWidth));
            childHeight = Math.Max(LayoutUtil.GetMinBoundsHeight((InvalidationManagerClient) element), Math.Min(maxChildHeight, childHeight));

            // Size the element
            element.SetLayoutBoundsSize(childWidth, childHeight);

            float x = cellX;
            switch (_horizontalAlign)
            {
                case HorizontalAlign.Right:
                    x += cellWidth - LayoutUtil.GetLayoutBoundsWidth((InvalidationManagerClient) element);
                break;
                case HorizontalAlign.Center:
                    // Make sure division result is integer - Math.floor() the result.
                    x = cellX + Mathf.Floor((cellWidth - LayoutUtil.GetLayoutBoundsWidth((InvalidationManagerClient) element)) / 2);
                break;
            }

            float y = cellY;
            switch (_verticalAlign)
            {
                case VerticalAlign.Bottom:
                    y += cellHeight - LayoutUtil.GetLayoutBoundsHeight((InvalidationManagerClient)element);
                break;
                case VerticalAlign.Middle:
                    // Make sure division result is integer - Math.floor() the result.
                y += Mathf.Floor((cellHeight - LayoutUtil.GetLayoutBoundsHeight((InvalidationManagerClient) element)) / 2);
                break;
            }

            // Position the element
            element.SetLayoutBoundsPosition(x, y);
        }

        /**
         *  
         *  Returns: Returns the x coordinate of the left edge for the specified column.
         */
        private float LeftEdge(int columnIndex)
        {
            return Mathf.Max(0, columnIndex * ((float)_columnWidth + _horizontalGap));
        }

        /**
         *  
         *  Returns: Returns the x coordinate of the right edge for the specified column.
         */
        private float RightEdge(int columnIndex)
        {
            return Mathf.Min(Target.ContentWidth, columnIndex * ((float)_columnWidth + _horizontalGap) + (float)_columnWidth);
        }

        /**
         *  
         *  Returns: Returns the y coordinate of the top edge for the specified row.
         */
        private float TopEdge(int rowIndex)
        {
            return Math.Max(0, rowIndex * ((float)_rowHeight + _verticalGap));
        }

        /**
         *  
         *  Returns: Returns the y coordinate of the bottom edge for the specified row.
         */
        private float BottomEdge(int rowIndex)
        {
            return Math.Min(Target.ContentHeight, rowIndex * ((float)_rowHeight + _verticalGap) + (float)_rowHeight);
        }

        //--------------------------------------------------------------------------
        //
        //  Overridden methods from LayoutBase
        //
        //--------------------------------------------------------------------------

        /**
         *  
         */
        override internal void ScrollPositionChanged()
        {
            base.ScrollPositionChanged();
            
            GroupBase layoutTarget = Target;
            if (null == layoutTarget)
                return;

            /*if (UseVirtualLayout)
                layoutTarget.InvalidateDisplayList();*/
        }

        internal override void Measure()
        {
            GroupBase layoutTarget = Target;
            if (null == layoutTarget)
                return;

            UpdateActualValues(
                null != layoutTarget.ExplicitWidth ? (float) layoutTarget.ExplicitWidth : 0, // MOD
                null != layoutTarget.ExplicitHeight ? (float)layoutTarget.ExplicitHeight : 0 // MOD
            );

            // For measure, any explicit overrides for rowCount and columnCount take precedence
            int columnCount = _requestedColumnCount != -1 ? Math.Max(1, _requestedColumnCount) : _columnCount;
            int rowCount = _requestedRowCount != -1 ? Math.Max(1, _requestedRowCount) : _rowCount;

            if (columnCount == 0)
                layoutTarget.MeasuredWidth = layoutTarget.MeasuredMinWidth = 0;
            else
            {
                layoutTarget.MeasuredWidth = Mathf.Ceil((float) (columnCount * (_columnWidth + _horizontalGap) - _horizontalGap));
                // measured min size is guaranteed to have enough columns to fit all elements
                layoutTarget.MeasuredMinWidth = Mathf.Ceil((float) (_columnCount * (_columnWidth + _horizontalGap) - _horizontalGap));
            }

            if (rowCount == 0)
                layoutTarget.MeasuredHeight = layoutTarget.MeasuredMinHeight = 0;        
            else
            {
                layoutTarget.MeasuredHeight = Mathf.Ceil((float) (rowCount * (_rowHeight + _verticalGap) - _verticalGap));
                // measured min size is guaranteed to have enough rows to fit all elements
                layoutTarget.MeasuredMinHeight = Mathf.Ceil((float) (_rowCount * (_rowHeight + _verticalGap) - _verticalGap));
            }
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
                if (navigationUnit == NavigationUnit.Up || navigationUnit == NavigationUnit.Left)
                    return arrowKeysWrapFocus ? maxIndex : -1;

                if (navigationUnit == NavigationUnit.Down || navigationUnit == NavigationUnit.Right)
                    return 0;    
            }    
                
            // Make sure currentIndex is within range
            bool inRows = _orientation == TileOrientation.Rows;
            currentIndex = Math.Max(0, Math.Min(maxIndex, currentIndex));

            // Find the current column and row
            int currentRow;
            int currentColumn;
            if (inRows)
            {
                // Is the TileLayout initialized with valid values?
                if (_columnCount == 0 || _rowHeight + _verticalGap == 0)
                    return currentIndex;
                        
                currentRow = currentIndex / _columnCount;
                currentColumn = currentIndex - currentRow * _columnCount;
            }
            else
            {
                // Is the TileLayout initialized with valid values?
                if (_rowCount == 0 || _columnWidth + _horizontalGap == 0)
                    return currentIndex;
        
                currentColumn = currentIndex / _rowCount;
                currentRow = currentIndex - currentColumn * _rowCount;
            }
            
            int newRow = currentRow;
            int newColumn = currentColumn;

            // Handle user input, almost all range checks are
            // performed after the calculations, at the end of the method.
            switch (navigationUnit)
            {
                case NavigationUnit.Left: 
                {
                    // If we are at the first column, can
                    // we go to the previous element (last column, previous row)?
                    if (newColumn == 0 && inRows && newRow > 0)
                    {
                        newRow--;
                        newColumn = _columnCount - 1;
                    }
                    else if (arrowKeysWrapFocus && newColumn == 0 && inRows && newRow == 0)
                    {
                        newRow = _rowCount - 1;
                        newColumn = _columnCount - 1;
                    }
                    else
                        newColumn--;
                    break;
                }

                case NavigationUnit.Right:
                {
                    // If we are at the last column, can
                    // we go to the next element (first column, next row)?
                    if (newColumn == _columnCount - 1 && inRows && newRow < _rowCount - 1)
                    {
                        newColumn = 0;
                        newRow++;
                    }
                    else if (arrowKeysWrapFocus && newColumn == _columnCount - 1 && inRows && newRow == _rowCount - 1)
                    {
                        newColumn = 0;
                        newRow = 0;
                    }
                    else
                        newColumn++;
                    break;
                } 

                case NavigationUnit.Up:
                {
                    // If we are at the first row, can we
                    // go to the previous element (previous column, last row)?
                    if (newRow == 0 && !inRows && newColumn > 0)
                    {
                        newColumn--;
                        newRow = _rowCount - 1;
                    }
                    else if (arrowKeysWrapFocus && newRow == 0 && !inRows && newColumn == 0)
                    {
                        newColumn = _columnCount - 1;
                        newRow = _rowCount - 1;
                    }
                    else
                        newRow--;
                    break; 
                }

                case NavigationUnit.Down:
                {
                    // If we are at the last row, can we
                    // go to the next element (next column, first row)?
                    if (newRow == _rowCount - 1 && !inRows && newColumn < _columnCount - 1)
                    {
                        newColumn++;
                        newRow = 0;
                    }
                    else if (arrowKeysWrapFocus && newRow == _rowCount - 1 && !inRows && newColumn == _columnCount - 1)
                    {
                        newColumn = 0;
                        newRow = 0;
                    }
                    else
                        newRow++;
                    break; 
                }

                case NavigationUnit.PageUp:
                case NavigationUnit.PageDown:
                {
                    // Ensure we have a valid scrollRect as we use it for calculations below
                    Rectangle scrollRect = GetScrollRect();
                    if (null == scrollRect)
                        scrollRect = new Rectangle(0, 0, Target.ContentWidth, Target.ContentHeight);
                     
                    if (inRows)
                    {
                        int firstVisibleRow = (int) Mathf.Ceil(scrollRect.Top / ((float)_rowHeight + _verticalGap));
                        int lastVisibleRow = (int) Mathf.Floor(scrollRect.Bottom / ((float)_rowHeight + _verticalGap));
                         
                        if (navigationUnit == NavigationUnit.PageUp)
                        {
                            // Is the current row visible, somewhere in the middle of the scrollRect?
                            if (firstVisibleRow < currentRow && currentRow <= lastVisibleRow)
                                newRow = firstVisibleRow;
                            else                             
                                newRow = 2 * firstVisibleRow - lastVisibleRow;
                        } 
                        else
                        {
                            // Is the current row visible, somewhere in the middle of the scrollRect?
                            if (firstVisibleRow <= currentRow && currentRow < lastVisibleRow)
                                newRow = lastVisibleRow;
                            else                             
                                newRow = 2 * lastVisibleRow - firstVisibleRow;
                        }
                    }
                    else
                    {
                        int firstVisibleColumn = (int) Mathf.Ceil(scrollRect.Left / ((float)_columnWidth + _horizontalGap));
                        int lastVisibleColumn = (int) Mathf.Floor(scrollRect.Right / ((float)_columnWidth + _horizontalGap));
                        
                        if (navigationUnit == NavigationUnit.PageUp)
                        {
                            // Is the current column visible, somewhere in the middle of the scrollRect?
                            if (firstVisibleColumn < currentColumn && currentColumn <= lastVisibleColumn)
                                newColumn = firstVisibleColumn;
                            else    
                                newColumn = 2 * firstVisibleColumn - lastVisibleColumn; 
                        }
                        else
                        {
                            // Is the current column visible, somewhere in the middle of the scrollRect?
                            if (firstVisibleColumn <= currentColumn && currentColumn < lastVisibleColumn)
                                newColumn = lastVisibleColumn;
                            else    
                                newColumn = 2 * lastVisibleColumn - firstVisibleColumn;
                        }
                    }
                    break; 
                }
                default: 
                    return base.GetNavigationDestinationIndex(currentIndex, navigationUnit, arrowKeysWrapFocus);
            }

            // Make sure rows and columns are within range
            newRow = Math.Max(0, Math.Min(_rowCount - 1, newRow));
            newColumn = Math.Max(0, Math.Min(_columnCount - 1, newColumn));

            // Calculate the new index based on orientation        
            if (inRows)  
            {
                // Make sure we don't return an index for an empty space in the last row.
                // newRow is guaranteed to be greater than zero:
                
                // Step 1: We can end up at the empty space in the last row if we moved right from
                // the last item.
                if (currentIndex == maxIndex && newColumn > currentColumn)
                    newColumn = currentColumn;
                    
                // Step 2: We can end up at the empty space in the last row if we moved down from
                // the previous row.    
                if (newRow == _rowCount - 1 && newColumn > maxIndex - _columnCount * (_rowCount - 1))
                    newRow--;

                return newRow * _columnCount + newColumn;
            }
            else
            {
                // Make sure we don't return an index for an empty space in the last column.
                // newColumn is guaranteed to be greater than zero:

                // Step 1: We can end up at the empty space in the last column if we moved down from
                // the last item.
                if (currentIndex == maxIndex && newRow > currentRow)
                    newRow = currentRow;

                // Step 2: We can end up at the empty space in the last column if we moved right from
                // the previous column.    
                if (newColumn == _columnCount - 1 && newRow > maxIndex - _rowCount * (_columnCount - 1))
                    newColumn--;

                return newColumn * _rowCount + newRow;
            }
        }

        /**
         *  
         */
        override internal void UpdateDisplayList(float unscaledWidth, float unscaledHeight)
        {
            GroupBase layoutTarget = Target;
            if (null == layoutTarget)
                return;

            calculateDisplayParameters((int) unscaledWidth, (int) unscaledHeight);
            //if (UseVirtualLayout)
            //    updateVirtualLayout((int) unscaledWidth, (int) unscaledHeight);  // re-calculateDisplayParameters()
            
            // Upper right hand corner of first (visibleStartIndex) tile/cell
            float xPos = _visibleStartX;  // 0 if useVirtualLayout=false
            float yPos = _visibleStartY;  // ...
                    
            // Use MajorDelta when moving along the major axis
            float xMajorDelta;
            float yMajorDelta;

            // Use MinorDelta when moving along the minor axis
            float xMinorDelta;
            float yMinorDelta;

            // Use counter and counterLimit to track when to move along the minor axis
            int counter = 0;
            int counterLimit;

            // Setup counterLimit and deltas based on orientation
            if (_orientation == TileOrientation.Rows)
            {
                counterLimit = _columnCount;
                xMajorDelta = (float) (_columnWidth + _horizontalGap);
                xMinorDelta = 0;
                yMajorDelta = 0;
                yMinorDelta = (float) (_rowHeight + _verticalGap);
            }
            else
            {
                counterLimit = _rowCount;
                xMajorDelta = 0;
                xMinorDelta = (float) (_columnWidth + _horizontalGap);
                yMajorDelta = (float) (_rowHeight + _verticalGap);
                yMinorDelta = 0;
            }

            for (int index = _visibleStartIndex; index <= _visibleEndIndex; index++)
            {
                ILayoutElement el = null; 
                //if (UseVirtualLayout)
                //{
                //    el = (ILayoutElement) layoutTarget.GetChildAt(index);
                //    if (el is IVisualElement)  // see updateVirtualLayout
                //        ((IVisualElement)el).Visible = true; 
                //}
                //else
                    el = (ILayoutElement) layoutTarget.GetChildAt(index);

                if (null == el || !el.IncludeInLayout)
                    continue;

                // To calculate the cell extents as integers, first calculate
                // the extents and then use Math.round()
                int cellX = (int) Mathf.Round(xPos);
                int cellY = (int) Mathf.Round(yPos);
                int cellWidth = (int) (Mathf.Round((float) (xPos + _columnWidth)) - cellX);
                int cellHeight = (int) (Mathf.Round((float) (yPos + _rowHeight)) - cellY);

                sizeAndPositionElement(el, cellX, cellY, cellWidth, cellHeight);

                // Move along the major axis
                xPos += xMajorDelta;
                yPos += yMajorDelta;

                // Move along the minor axis
                if (++counter >= counterLimit)
                {
                    counter = 0;
                    if (_orientation == TileOrientation.Rows)
                    {
                        xPos = 0;
                        yPos += yMinorDelta;
                    }
                    else
                    {
                        xPos += xMinorDelta;
                        yPos = 0;
                    }
                }
            }

            // Make sure that if the content spans partially over a pixel to the right/bottom,
            // the content size includes the whole pixel.
            layoutTarget.SetContentSize(Mathf.Ceil((float) (_columnCount * (_columnWidth + _horizontalGap) - _horizontalGap)),
                                        Mathf.Ceil((float) (_rowCount * (_rowHeight + _verticalGap) - _verticalGap)));

            // Reset the cache
            //if (!UseVirtualLayout)
                _tileWidthCached = _tileHeightCached = null;
            _numElementsCached = -1;
            
            // No getVirtualElementAt() during measure, see calculateVirtualTileSize()
            //if (UseVirtualLayout)
            //    _visibleStartIndex = _visibleEndIndex = -1;        

            // If actual values have chnaged, notify listeners
            DispatchEventsForActualValueChanges();
        }

        /**
         *  
         */
        override internal Rectangle GetElementBounds(int index)
        {
            //if (!UseVirtualLayout)
            //    return base.GetElementBounds(index);

            GroupBase g = Target;
            if (null == g || (index < 0) || (index >= g.NumberOfChildren)) 
                return null;

            int col;
            int row;
            if (_orientation == TileOrientation.Rows)
            {
                col = index % _columnCount;
                row = index / _columnCount;
            }
            else
            {
                col = index / _rowCount;
                row = index%_rowCount;
            }
            return new Rectangle(LeftEdge(col), TopEdge(row), (float) _columnWidth, (float) _rowHeight);
        }

        /**
         *  
         */
        override internal Rectangle GetElementBoundsLeftOfScrollRect(Rectangle scrollRect)
        {
            var bounds = new Rectangle();
            // Find the column that spans or is to the left of the scrollRect left edge.
            int column = (int) Math.Floor((double) ((scrollRect.Left - 1) / (_columnWidth + _horizontalGap)));
            bounds.Left = LeftEdge(column);
            bounds.Right = RightEdge(column);
            return bounds;
        }

        /**
         *  
         */
        override internal Rectangle GetElementBoundsRightOfScrollRect(Rectangle scrollRect)
        {
            var bounds = new Rectangle();
            // Find the column that spans or is to the right of the scrollRect right edge.
            int column = (int) Math.Floor((double) ((scrollRect.Right + 1 + _horizontalGap) / (_columnWidth + _horizontalGap)));
            bounds.Left = LeftEdge(column);
            bounds.Right = RightEdge(column);
            return bounds;
        }

        /**
         *  
         */
        override internal Rectangle GetElementBoundsAboveScrollRect(Rectangle scrollRect)
        {
            var bounds = new Rectangle();
            // Find the row that spans or is above the scrollRect top edge
            int row = (int) Math.Floor((double) ((scrollRect.Top - 1) / (_rowHeight + _verticalGap)));
            bounds.Top = TopEdge(row);
            bounds.Bottom = BottomEdge(row);
            return bounds;
        }

        /**
         *  
         */
        override internal Rectangle GetElementBoundsBelowScrollRect(Rectangle scrollRect)
        {
            var bounds = new Rectangle();
            // Find the row that spans or is below the scrollRect bottom edge
            int row = (int) Math.Floor((double) ((scrollRect.Bottom + 1 + _verticalGap) / (_rowHeight + _verticalGap)));
            bounds.Top = TopEdge(row);
            bounds.Bottom = BottomEdge(row);
            return bounds;
        }
    }
}
