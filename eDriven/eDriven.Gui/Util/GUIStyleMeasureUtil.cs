using System;
using UnityEngine;

namespace eDriven.Gui.Util
{
    /// <summary>
    /// Utility class for measuring GUIStyles
    /// </summary>
// ReSharper disable InconsistentNaming
    public class GUIStyleMeasureUtil
// ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// Measures the bounds needed for displaying all aspects of the GUIStyle: <br/>
        /// 1. fixedWidth, fixedHeight<br/>
        /// 2. normal.background, hover.background, active.background, focused.background, onNormal.background, onHover.background, onActive.background, onFocused.background
        /// </summary>
        /// <param name="style"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void MeasureStyle(GUIStyle style, out float width, out float height)
        {
            float w = 0;
            float h = 0;

            if (null == style)
                throw new Exception(string.Format("No style defined"));

            // fixedWidth
            if (style.fixedWidth > 0)  // from fixedWidth
                w = style.fixedWidth;
            // fixedHeight
            if (style.fixedHeight > 0) // from fixedHeight
                h = style.fixedHeight;

            if (null != style.normal && null != style.normal.background) // from normal texture background
            {
                //if (style.normal.background.width > 0)
                w = Math.Max(w, style.normal.background.width);
                //if (style.normal.background.height > 0)
                h = Math.Max(h, style.normal.background.height);
            }

            if (null != style.hover && null != style.hover.background) // from hover texture background
            {
                w = Math.Max(w, style.hover.background.width);
                h = Math.Max(h, style.hover.background.height);
            }

            if (null != style.active && null != style.active.background) // from active texture background
            {
                w = Math.Max(w, style.active.background.width);
                h = Math.Max(h, style.active.background.height);
            }

            if (null != style.focused && null != style.focused.background) // from focused texture background
            {
                w = Math.Max(w, style.focused.background.width);
                h = Math.Max(h, style.focused.background.height);
            }

            if (null != style.onNormal && null != style.onNormal.background) // from onNormal texture background
            {
                w = Math.Max(w, style.onNormal.background.width);
                h = Math.Max(h, style.onNormal.background.height);
            }

            if (null != style.onHover && null != style.onHover.background) // from onHover texture background
            {
                w = Math.Max(w, style.onHover.background.width);
                h = Math.Max(h, style.onHover.background.height);
            }

            if (null != style.onActive && null != style.onActive.background) // from onActive texture background
            {
                w = Math.Max(w, style.onActive.background.width);
                h = Math.Max(h, style.onActive.background.height);
            }

            if (null != style.onFocused && null != style.onFocused.background) // from onFocused texture background
            {
                w = Math.Max(w, style.onFocused.background.width);
                h = Math.Max(h, style.onFocused.background.height);
            }

            width = w;
            height = h;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void MeasureContent(GUIStyle style, GUIContent content, out float width, out float height)
        {
            //if (comp is Label)
            //    Debug.Log(comp + " -> MeasureContent()");

            if (null == style)
                throw new Exception(string.Format("Cannot measure because style not set. Consider calling ValidateNow() on component after the addition."));

            Vector2 size = style.CalcSize(content);

            width = size.x;
            height = size.y;
        }
    }
}
