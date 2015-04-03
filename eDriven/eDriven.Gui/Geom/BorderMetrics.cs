#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using eDriven.Core.Geom;
using UnityEngine;

namespace eDriven.Gui.Geom
{
    /// <summary>
    /// The class used for specifying edges (padding or margins)
    /// </summary>
    /// <remarks>Coded by Danko Kozar</remarks>
    [Serializable]
    public class BorderMetrics : ICloneable
    {
        /// <summary>
        /// Zero metrics
        /// </summary>
        public static readonly BorderMetrics Empty = new BorderMetrics(0, 0, 0, 0);
        
        #region Properties

        /// <summary>
        /// The value representing the left edge
        /// </summary>
        public int Left;

        /// <summary>
        /// The value representing the right edge
        /// </summary>
        public int Right;

        /// <summary>
        /// The value representing the top edge
        /// </summary>
        public int Top;

        /// <summary>
        /// The value representing the bottom edge
        /// </summary>
        public int Bottom;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BorderMetrics()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BorderMetrics(int width)
        {
            Left = Right = Top = Bottom = width;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BorderMetrics(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the width of the texture needed for pixel-perfect rendering
        /// </summary>
        /// <returns></returns>
        public int GetRenderingWidth()
        {
            return Left + 2 + Right;
        }

        /// <summary>
        /// Gets the height of the texture needed for pixel-perfect rendering
        /// </summary>
        /// <returns></returns>
        public int GetRenderingHeight()
        {
            return Top + 2 + Bottom;
        }

        /////<summary>
        ///// Expands the border by given values
        /////</summary>
        /////<param name="left"></param>
        /////<param name="right"></param>
        /////<param name="top"></param>
        /////<param name="bottom"></param>
        /////<returns></returns>
        //public BorderMetrics Expand(int left, int right, int top, int bottom)
        //{
        //    BorderMetrics r = (BorderMetrics)Clone();
        //    r.Left += left;
        //    r.Left = Math.Max(r.Left, 0);
        //    r.Right += right;
        //    r.Right = Math.Max(r.Right, 0);
        //    r.Top += top;
        //    r.Top = Math.Max(r.Top, 0);
        //    r.Bottom += bottom;
        //    r.Bottom = Math.Max(r.Bottom, 0);
        //    return r;
        //}

        /// <summary>
        /// Returns the minimum between all edges
        /// </summary>
        public int Minimum
        {
            get
            {
                return Mathf.Min(Left, Right, Top, Bottom);
            }
        }

        /// <summary>
        /// Returns maximum between all edges
        /// </summary>
        public int Maximum
        {
            get
            {
                return Mathf.Max(Left, Right, Top, Bottom);
            }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("Left: {0}, Right: {1}, Top: {2}, Bottom: {3}", Left, Right, Top, Bottom);
        }

        #endregion

        #region Equals

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(EdgeMetrics other)
        {
            if (ReferenceEquals(null, other)) return false;
            //if (ReferenceEquals(this, other)) return true;
            return other.Left == Left && other.Right == Right && other.Top == Top && other.Bottom == Bottom;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            //if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(EdgeMetrics)) return false;
            return Equals((EdgeMetrics)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Left.GetHashCode();
                result = (result * 397) ^ Right.GetHashCode();
                result = (result * 397) ^ Top.GetHashCode();
                result = (result * 397) ^ Bottom.GetHashCode();
                return result;
            }
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            EdgeMetrics em = new EdgeMetrics {Left = Left, Right = Right, Top = Top, Bottom = Bottom};

            return em;
        }

        #endregion

        /////<summary>
        ///// Converts to RectOffset
        /////</summary>
        /////<returns></returns>
        //public RectOffset ToRectOffset()
        //{
        //    return new RectOffset(Left, Right, Top, Bottom);
        //}

        ///<summary>
        /// Converts to RectOffset
        ///</summary>
        ///<returns></returns>
        public RectOffset ToGUIStyleBorder()
        {
            return new RectOffset(Left + 1, Right + 1, Top + 1, Bottom + 1);
        }

    }
}