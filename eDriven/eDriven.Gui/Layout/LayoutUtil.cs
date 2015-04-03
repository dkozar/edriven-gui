using System;
using eDriven.Gui.Components;

namespace eDriven.Gui.Layout
{
    /// <summary>
    /// Layout utility
    /// </summary>
    public static class LayoutUtil
    {
// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming
        internal static float DEFAULT_MAX_WIDTH = 10000;
        internal static float DEFAULT_MAX_HEIGHT = 10000;
// ReSharper restore InconsistentNaming
// ReSharper restore ConvertToConstant.Local

        internal static float GetPreferredBoundsWidth(InvalidationManagerClient obj)
        {
            return obj.GetExplicitOrMeasuredWidth();
        }

        internal static float GetPreferredBoundsHeight(InvalidationManagerClient obj)
        {
            return obj.GetExplicitOrMeasuredHeight();
        }

        public static float GetMinBoundsWidth(InvalidationManagerClient obj)
        {
            float minWidth;
            if (null != obj.ExplicitMinWidth)
            {
                minWidth = (float) obj.ExplicitMinWidth;
            }
            else
            {
                //minWidth = null == obj.MeasuredMinWidth ? 0 : obj.MeasuredMinWidth;
                minWidth = obj.MeasuredMinWidth; // TODO: eventual bug here
                if (null != obj.ExplicitMaxWidth)
                    minWidth = Math.Min(minWidth, (float)obj.ExplicitMaxWidth);
            }
            return minWidth;
        }

        public static float GetMinBoundsHeight(InvalidationManagerClient obj)
        {
            // explicit trumps explicitMin trumps measuredMin.
            // measuredMin is restricted by explicitMax.
            float minHeight;
            if (null != obj.ExplicitMinHeight)
            {
                minHeight = (float) obj.ExplicitMinHeight;
            }
            else
            {
                //minHeight = isNaN(obj.measuredMinHeight) ? 0 : obj.measuredMinHeight;
                minHeight = obj.MeasuredMinHeight;
                if (null != obj.ExplicitMaxHeight)
                    minHeight = Math.Min(minHeight, (float)obj.ExplicitMaxHeight);
            }

            return minHeight;
        }

        internal static float GetMaxBoundsWidth(InvalidationManagerClient obj)
        {
            // explicit trumps explicitMax trumps Number.MAX_VALUE.
            float maxWidth = obj.ExplicitMaxWidth ?? DEFAULT_MAX_WIDTH;

            return maxWidth;
        }

        internal static float GetMaxBoundsHeight(InvalidationManagerClient obj)
        {
            // explicit trumps explicitMax trumps Number.MAX_VALUE.
            float maxHeight = obj.ExplicitMaxHeight ?? DEFAULT_MAX_HEIGHT;

            return maxHeight;
        }

        internal static float GetBoundsXAtSize(InvalidationManagerClient obj, float? width, float? height)
        {
            // explicit trumps explicitMax trumps Number.MAX_VALUE.
            //return GetMaxBoundsWidth(obj);
            return obj.X;
        }

        internal static float GetBoundsYAtSize(InvalidationManagerClient obj, float? width, float? height)
        {
            // explicit trumps explicitMax trumps Number.MAX_VALUE.
            //return GetMaxBoundsHeight(obj);
            return obj.Y;
        }

        internal static float GetLayoutBoundsWidth(InvalidationManagerClient obj)
        {
            return obj.Width;
        }

        internal static float GetLayoutBoundsHeight(InvalidationManagerClient obj)
        {
            return obj.Height;
        }

        internal static float GetLayoutBoundsX(InvalidationManagerClient obj)
        {
            return obj.X;
        }

        internal static float GetLayoutBoundsY(InvalidationManagerClient obj)
        {
            return obj.Y;
        }

        internal static void SetLayoutBoundsPosition(InvalidationManagerClient obj, float x, float y)
        {
            obj.Move(x, y);
        }

        internal static void SetLayoutBoundsSize(InvalidationManagerClient obj, float? width, float? height)
        {
            if (null == width)
                width = GetPreferredBoundsWidth(obj);
            if (null == height)
                height = GetPreferredBoundsHeight(obj);

            obj.SetActualSize((float) width, (float) height);
        }

        internal static float? ParseConstraintValue(object value)
        {
            if (value is float)
                return (float)value;

            if (value is int)
            {
                return (int)value;
            }
            
            //var str:String = value as String;
            //if (!str)
            //    return NaN;
            
            //var result:Array = parseConstraintExp(str);
            //if (!result || result.length < 1)
            //    return NaN;

            //return result[0];
            return null;
        }
    }
}
