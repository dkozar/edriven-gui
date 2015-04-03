//using System;
//using eDriven.Gui.Components.Core;

//namespace eDriven.Gui.Components.Core
//{
//    public class EdgeMetrics : ICloneable
//    {
//        public float Left;
//        public float Right;
//        public float Top;
//        public float Bottom;

//        public EdgeMetrics()
//        {
//        }

//        public EdgeMetrics(float left, float right, float top, float bottom)
//        {
//            Left = left;
//            Right = right;
//            Top = top;
//            Bottom = bottom;
//        }

//        public static EdgeMetrics Empty
//        {
//            get
//            {
//                return new EdgeMetrics(0, 0, 0, 0);
//            }
//        }

//        public override string ToString()
//        {
//            return string.Format("Left: {0}, Right: {1}, Top: {2}, Bottom: {3}", Left, Right, Top, Bottom);
//        }

//        /// <summary>
//        /// Creates a new object that is a copy of the current instance.
//        /// </summary>
//        /// <returns>
//        /// A new object that is a copy of this instance.
//        /// </returns>
//        /// <filterpriority>2</filterpriority>
//        public object Clone()
//        {
//            EdgeMetrics em = new EdgeMetrics();
//            em.Left = Left;
//            em.Right = Right;
//            em.Top = Top;
//            em.Bottom = Bottom;

//            return em;
//        }
//    }
//}