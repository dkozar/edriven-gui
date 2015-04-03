using System;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.Layout
{
    ///<summary>
    ///</summary>
    public sealed class AbsoluteLayout : LayoutBase
    {
        #region Helper

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
        }
        
        /**
         *  Returns: Returns the maximum value for an element's dimension so that the component doesn't
         *  spill out of the container size. Calculations are based on the layout rules.
         *  Pass in unscaledWidth, hCenter, left, right, childX to get a maxWidth value.
         *  Pass in unscaledHeight, vCenter, top, bottom, childY to get a maxHeight value.
         */
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

        #endregion

        ///<summary>
        ///</summary>
        override internal void Measure()
        {
            //base.Measure();

            GroupBase layoutTarget = Target;
            if (null == layoutTarget)
                return;

            float width = 0;
            float height = 0;
            float minWidth = 0;
            float minHeight = 0;

            //Debug.Log("Target: " + Target); // Error!
            int count = layoutTarget.NumberOfChildren;

            for (int i = 0; i < count; i++)
            {
                ILayoutElement child = (ILayoutElement)Target.GetChildAt(i);
                if (null == child || !child.IncludeInLayout)
                    continue;

                float? left = LayoutUtil.ParseConstraintValue(child.Left);
                float? right = LayoutUtil.ParseConstraintValue(child.Right);
                float? top = LayoutUtil.ParseConstraintValue(child.Top);
                float? bottom = LayoutUtil.ParseConstraintValue(child.Bottom);
                float? hCenter = LayoutUtil.ParseConstraintValue(child.HorizontalCenter);
                float? vCenter = LayoutUtil.ParseConstraintValue(child.VerticalCenter);

                // Extents of the element - how much additional space (besides its own width/height)
                // the element needs based on its constraints.
                float extX;
                float extY;

                if (null != left && null != right)
                {
                    // If both left & right are set, then the extents is always
                    // left + right so that the element is resized to its preferred
                    // size (if it's the one that pushes out the default size of the container).
                    extX = (float)(left + right);
                }
                else if (null != hCenter)
                {
                    // If we have horizontalCenter, then we want to have at least enough space
                    // so that the element is within the parent container.
                    // If the element is aligned to the left/right edge of the container and the
                    // distance between the centers is hCenter, then the container width will be
                    // parentWidth = 2 * (abs(hCenter) + elementWidth / 2)
                    // <=> parentWidth = 2 * abs(hCenter) + elementWidth
                    // Since the extents is the additional space that the element needs
                    // extX = parentWidth - elementWidth = 2 * abs(hCenter)
                    extX = Math.Abs((float)hCenter) * 2;
                }
                else if (null != left || null != right)
                {
                    extX = left ?? 0;
                    extX += right ?? 0;
                }
                else
                {
                    extX = LayoutUtil.GetBoundsXAtSize((InvalidationManagerClient) child, null, null);
                }

                if (null != top && null != bottom)
                {
                    // If both top & bottom are set, then the extents is always
                    // top + bottom so that the element is resized to its preferred
                    // size (if it's the one that pushes out the default size of the container).
                    extY = (float)(top + bottom);
                }
                else if (null != vCenter)
                {
                    // If we have verticalCenter, then we want to have at least enough space
                    // so that the element is within the parent container.
                    // If the element is aligned to the top/bottom edge of the container and the
                    // distance between the centers is vCenter, then the container height will be
                    // parentHeight = 2 * (abs(vCenter) + elementHeight / 2)
                    // <=> parentHeight = 2 * abs(vCenter) + elementHeight
                    // Since the extents is the additional space that the element needs
                    // extY = parentHeight - elementHeight = 2 * abs(vCenter)
                    extY = Math.Abs((float)vCenter) * 2;
                }
                else if (null != top || null != bottom)
                {
                    extY = top ?? 0;
                    extY += bottom ?? 0;
                }
                else
                {
                    extY = LayoutUtil.GetBoundsYAtSize((InvalidationManagerClient)child, null, null);
                }

                float preferredWidth = LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient)child);
                float preferredHeight = LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient)child);

                width = Math.Max(width, extX + preferredWidth);
                height = Math.Max(height, extY + preferredHeight);

                // Find the minimum default extents, we take the minimum width/height only
                // when the element size is determined by the parent size
                float elementMinWidth =
                    ConstraintsDetermineWidth(child) ? LayoutUtil.GetMinBoundsWidth((InvalidationManagerClient)child) :
                                                                                                                           preferredWidth;
                float elementMinHeight =
                    ConstraintsDetermineHeight(child) ? LayoutUtil.GetMinBoundsHeight((InvalidationManagerClient)child) :
                                                                                                                             preferredHeight;

                minWidth = Math.Max(minWidth, extX + elementMinWidth);
                minHeight = Math.Max(minHeight, extY + elementMinHeight);
            }

            Target.MeasuredWidth = Mathf.Ceil(Math.Max(width, minWidth));
            Target.MeasuredHeight = Mathf.Ceil(Math.Max(height, minHeight));
            Target.MeasuredMinWidth = Mathf.Ceil(minWidth);
            Target.MeasuredMinHeight = Mathf.Ceil(minHeight);

//            if (((Component)Target.Owner).Id == "test")
//                Debug.Log("############### Measured: " + Target.MeasuredWidth + ", " + Target.MeasuredHeight);

            //Debug.Log("     ---> " + Target + " Measured; " + Target.MeasuredWidth + ", " + Target.MeasuredHeight + "; Layout: " + Target.Layout + "; NestLevel: " + Target.NestLevel);

            //if (Target.Id == "stf") // skin
            //    Debug.Log("############### stf Measured: " + Target.MeasuredWidth + ", " + Target.MeasuredHeight + "; Min: " + Target.MeasuredMinWidth + ", " + Target.MeasuredMinHeight + "; NestLevel: " + Target.NestLevel);

            //if (Target.Parent.Id == "stf") // skin
            //    Debug.Log("############### stf2 Measured: " + Target.MeasuredWidth + ", " + Target.MeasuredHeight + "; Min: " + Target.MeasuredMinWidth + ", " + Target.MeasuredMinHeight + "; NestLevel: " + Target.NestLevel);
            //if (Target.Parent.Id == "ns1") // skin
            //    Debug.Log("############### ns1 Measured: " + Target.MeasuredWidth + ", " + Target.MeasuredHeight + "; Min: " + Target.MeasuredMinWidth + ", " + Target.MeasuredMinHeight + "; NestLevel: " + Target.NestLevel);
        }

        ///<summary>
        ///</summary>
        ///<param name="width"></param>
        ///<param name="height"></param>
        override internal void UpdateDisplayList(float width, float height)
        {
            //if (((Component)Target.Owner).Id == "test")
            //    Debug.Log("AbsoluteLayout -> UpdateDisplayList: " + width + ", " + height);

            //base.UpdateDisplayList(width, height);

            GroupBase layoutTarget = Target;
            if (null == layoutTarget)
                return;

            int count = layoutTarget.NumberOfChildren;
            float maxX = 0;
            float maxY = 0;
            for (int i = 0; i < count; i++)
            {
                //Debug.Log("layoutTarget.GetChildAt(i): " + layoutTarget.GetChildAt(i));

                ILayoutElement layoutElement = (ILayoutElement)layoutTarget.GetChildAt(i);

                //if (layoutElement is LoadingMaskBase)
                //    Debug.Log(string.Format("*** A) layoutElement is LoadingMaskBase: layoutElement.IncludeInLayout: {0};", layoutElement.IncludeInLayout));

                if (null == layoutElement || !layoutElement.IncludeInLayout)
                    continue;

                //if (layoutElement is LoadingMaskBase)
                //    Debug.Log(string.Format("*** B) layoutElement is LoadingMaskBase: layoutElement.IncludeInLayout: {0};", layoutElement.IncludeInLayout));

                float? left = LayoutUtil.ParseConstraintValue(layoutElement.Left);
                float? right = LayoutUtil.ParseConstraintValue(layoutElement.Right);
                float? top = LayoutUtil.ParseConstraintValue(layoutElement.Top);
                float? bottom = LayoutUtil.ParseConstraintValue(layoutElement.Bottom);
                float? hCenter = LayoutUtil.ParseConstraintValue(layoutElement.HorizontalCenter);
                float? vCenter = LayoutUtil.ParseConstraintValue(layoutElement.VerticalCenter);
                float? percentWidth = layoutElement.PercentWidth;
                float? percentHeight = layoutElement.PercentHeight;

                float? elementMaxWidth = null;
                float? elementMaxHeight = null;

                // Calculate size
                float? childWidth = null;
                float? childHeight = null;

                if (null != percentWidth)
                {
                    var availableWidth = width;
                    if (null != left)
                        availableWidth -= (float)left;
                    if (null != right)
                        availableWidth -= (float)right;

                    childWidth = (float?)Math.Round(availableWidth * Math.Min((float)percentWidth * 0.01f, 1f));
                    elementMaxWidth = Math.Min(LayoutUtil.GetMaxBoundsWidth(layoutElement as InvalidationManagerClient),
                                               MaxSizeToFitIn(width, hCenter, left, right, LayoutUtil.GetLayoutBoundsX(layoutElement as InvalidationManagerClient)));
                }
                else if (null != left && null != right)
                {
                    childWidth = width - right - left;
                }

                if (null != percentHeight)
                {
                    float availableHeight = height;
                    if (null != top)
                        availableHeight -= (float)top;
                    if (null != bottom)
                        availableHeight -= (float)bottom;

                    childHeight = (float?)Math.Round(availableHeight * Math.Min((float)percentHeight * 0.01, 1));
                    elementMaxHeight = Math.Min(LayoutUtil.GetMaxBoundsHeight(layoutElement as InvalidationManagerClient),
                                                MaxSizeToFitIn(height, vCenter, top, bottom, LayoutUtil.GetLayoutBoundsY(layoutElement as InvalidationManagerClient)));
                }
                else if (null != top && null != bottom)
                {
                    childHeight = height - bottom - top;
                }

                // Apply min and max constraints, make sure min is applied last. In the cases
                // where childWidth and childHeight are NaN, setLayoutBoundsSize will use preferredSize
                // which is already constrained between min and max.
                if (null != childWidth)
                {
                    if (null == elementMaxWidth)
                        elementMaxWidth = LayoutUtil.GetMaxBoundsWidth(layoutElement as InvalidationManagerClient);
                    childWidth = Math.Max(LayoutUtil.GetMinBoundsWidth(layoutElement as InvalidationManagerClient), Math.Min((float)elementMaxWidth, (float)childWidth));
                }
                if (null != childHeight)
                {
                    if (null == elementMaxHeight)
                        elementMaxHeight = LayoutUtil.GetMaxBoundsHeight(layoutElement as InvalidationManagerClient);
                    childHeight = Math.Max(LayoutUtil.GetMinBoundsHeight(layoutElement as InvalidationManagerClient), Math.Min((float)elementMaxHeight, (float)childHeight));
                }

                // Set the size.
                LayoutUtil.SetLayoutBoundsSize(layoutElement as InvalidationManagerClient, childWidth, childHeight);
                float elementWidth = LayoutUtil.GetLayoutBoundsWidth(layoutElement as InvalidationManagerClient);
                float elementHeight = LayoutUtil.GetLayoutBoundsHeight(layoutElement as InvalidationManagerClient);

                float childX;
                float childY;

                // Horizontal position
                if (null != hCenter)
                    childX = (float)Math.Round((width - elementWidth) / 2 + (float)hCenter);
                else if (null != left)
                    childX = (float)left;
                else if (null != right)
                    childX = (float)(width - elementWidth - right);
                else
                    childX = LayoutUtil.GetLayoutBoundsX(layoutElement as InvalidationManagerClient);

                // Vertical position
                if (null != vCenter)
                    childY = (float)Math.Round((height - elementHeight) / 2 + (float)vCenter);
                else if (null != top)
                    childY = (float)top;
                else if (null != bottom)
                    childY = (float)(height - elementHeight - bottom);
                else
                    childY = LayoutUtil.GetLayoutBoundsY(layoutElement as InvalidationManagerClient);

                // Set position
                //LayoutUtil.SetLayoutBoundsPosition(layoutElement as InvalidationManagerClient, childX, childY);
                layoutElement.SetLayoutBoundsPosition(childX, childY);

                // update content limits
                maxX = Math.Max(maxX, childX + elementWidth);
                maxY = Math.Max(maxY, childY + elementHeight);
            }

            // Make sure that if the content spans partially over a pixel to the right/bottom,
            // the content size includes the whole pixel.
            layoutTarget.SetContentSize(Mathf.Ceil(maxX), Mathf.Ceil(maxY));
        }
    }
}