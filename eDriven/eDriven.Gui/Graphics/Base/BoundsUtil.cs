using System;
using eDriven.Core.Geom;
using UnityEngine;

namespace eDriven.Gui.Graphics.Base
{
    public class BoundsUtil
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        public static Rectangle CalculateBounds(float parentWidth, float parentHeight, float? left, float? right, float? top, float? bottom, float? width, float? height)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log(
                string.Format("CalculateBounds parentWidth: {0}; parentHeight: {1}; left: {2}; right: {3}; top: {4}; bottom: {5}; width: {6}; height: {7};",
                parentWidth, parentHeight, left, right, top, bottom, width, height));
#endif      

            float x = left ?? 0;
            float y = top ?? 0;
            float w;
            float h;

            /*Debug.Log("parentWidth: " + parentWidth);
            Debug.Log("left: " + left);
            Debug.Log("right: " + right);*/

            if (null != width) // explicit width defined
                w = (int)width;
            else
            {
                w = parentWidth;
                if (null != left)
                    w -= (float) left;
                if (null != right)
                    w -= (float) right;

                w = Math.Max(0, w);
            }

            if (null != height) // explicit width defined
                h = (int)height;
            else
            {
                h = parentHeight;
                if (null != top)
                    h -= (float)top;
                if (null != bottom)
                    h -= (float)bottom;

                h = Math.Max(0, h);
            }

            Rectangle b = new Rectangle(x, y, w, h);
            //Debug.Log(b);

            return b;
        }
    }
}
