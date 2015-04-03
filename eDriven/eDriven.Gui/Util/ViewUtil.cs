using eDriven.Core.Geom;
using UnityEngine;

namespace eDriven.Gui.Util
{
    public class ViewUtil
    {
        public static Rect OffsetRect(Rect rectangle, float offsetLeft, float offsetTop)
        {
            float left = rectangle.xMin + offsetLeft;
            float top = rectangle.yMin + offsetTop;
            return new Rect(left, top, rectangle.width, rectangle.height);
        }

        public static Rect CenterRect(Rect rectangle)
        {
            return new Rect((Screen.width - rectangle.width) / 2, (Screen.height - rectangle.height) / 2, rectangle.width, rectangle.height);
        }

        public static Rect CenterRect(Rect inner, Rect outer)
        {
            //Debug.Log("inner: " + inner + "; outer: " + outer);
            return new Rect((outer.width - inner.width) / 2, (outer.height - inner.height) / 2, inner.width, inner.height);
        }

        public static Rectangle CenterRect(Rectangle inner, Rectangle outer)
        {
            //Debug.Log("inner: " + inner + "; outer: " + outer);
            return new Rectangle((outer.Width - inner.Width) / 2, (outer.Height - inner.Height) / 2, inner.Width, inner.Height);
        }

        #region _crap

        //public static Rect CenterAroundPoint(Rect rectangle, Rect outer, Vector2 point)
        //{
        //    return new Rect(outer.X + point.X - rectangle.Width / 2, outer.Y + point.Y - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        //}

        //public static Rect Expand(Rect rectangle, float expandLeft, float expandRight, float expandTop, float expandBottom)
        //{
        //    float Left = rectangle.X - expandLeft;
        //    float Top = rectangle.Y - expandTop;
        //    float Width = rectangle.Width + expandLeft + expandRight;
        //    float Height = rectangle.Height + expandTop + expandBottom;
        //    return new Rect(Left, Top, Width, Height); 
        //}

        //public static Rect Expand(Rect rectangle, float expand)
        //{
        //    return Expand(rectangle, expand, expand, expand, expand);
        //}

        //public static Rect Collapse(Rect rectangle, float collapseLeft, float collapseRight, float collapseTop, float collapseBottom)
        //{
        //    float Left = rectangle.X + collapseLeft;
        //    float Top = rectangle.Y + collapseTop;
        //    float Width = Mathf.Max(rectangle.Width - collapseLeft - collapseRight, 0);
        //    float Height = Mathf.Max(rectangle.Height - collapseTop - collapseBottom, 0);
        //    return new Rect(Left, Top, Width, Height);
        //}

        //public static Rect Collapse(Rect rectangle, float expand)
        //{
        //    return Collapse(rectangle, expand, expand, expand, expand);
        //}

        //public static Rect Delta(Rect rectangle, float deltaX, float deltaY)
        //{
        //    return new Rect(rectangle.X + deltaX, rectangle.Y + deltaY, rectangle.Width, rectangle.Height);
        //}

        #endregion

        public static Vector3 FlipY(Vector3 v)
        {
            return new Vector3(v.x, Screen.height - v.y, v.z);
        }

// ReSharper disable InconsistentNaming
        public static Rect NULL_RECT = new Rect(0, 0, 0, 0);
// ReSharper restore InconsistentNaming
    }
}