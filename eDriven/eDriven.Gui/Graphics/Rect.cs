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

using eDriven.Core.Geom;
using eDriven.Gui.Graphics.Base;
using UnityEngine;

namespace eDriven.Gui.Graphics
{
    public sealed class Rect : GraphicsBase
    {
        private bool _isFill;
        private bool _isStroke;

        private Color _color;
        private readonly Point _point = new Point();

        #region Constructor

        public Rect()
        {
        }

        public Rect(params GraphicOption[] options) : base(options)
        {
        }

        public Rect(IFill fill, params GraphicOption[] options) : base(fill, options)
        {
        }

        public Rect(IStroke stroke, params GraphicOption[] options) : base(stroke, options)
        {
        }

        public Rect(IFill fill, IStroke stroke) : base(fill, stroke)
        {
        }

        public Rect(int width, int height, params GraphicOption[] options) : base(width, height, options)
        {
        }

        public Rect(int width, int height, IStroke stroke, params GraphicOption[] options) : base(width, height, stroke, options)
        {
        }

        public Rect(int width, int height, IFill fill, params GraphicOption[] options) : base(width, height, fill, options)
        {
        }

        public Rect(int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(width, height, fill, stroke, options)
        {
        }

        public Rect(int x, int y, int width, int height, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, stroke, options)
        {
        }

        public Rect(int x, int y, int width, int height, IFill fill, params GraphicOption[] options) : base(x, y, width, height, fill, options)
        {
        }

        public Rect(int x, int y, int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, fill, stroke, options)
        {
        }

        public Rect(int x, int y, int width, int height, params GraphicOption[] options) : base(x, y, width, height, options)
        {
        }

        public Rect(IFill fill, IStroke stroke, params GraphicOption[] options) : base(fill, stroke, options)
        {
        }

        #endregion

        public override void Draw()
        {
            //Debug.Log("Drawing rect");

            if (null == Fill && null == Stroke)
                return;

            base.Draw();

            Rectangle fillRect = (Rectangle)Bounds.Clone();

            bool hasStroke = false;
            if (null != Stroke) {
                //Debug.Log("Stroke.Border: " + Stroke.Border);
                fillRect = fillRect.Collapse(Stroke.Border.Left, Stroke.Border.Right, Stroke.Border.Top, Stroke.Border.Bottom);
                //Debug.Log("    fillRect: " + fillRect);
                hasStroke = true;
            }

            //Debug.Log("fillRect: " + fillRect);

            //Debug.Log("Bounds.Left: " + Bounds.Left);
            //Debug.Log("Bounds.Right: " + Bounds.Right);
            //Debug.Log("Bounds.Top: " + Bounds.Top);
            //Debug.Log("Bounds.Bottom: " + Bounds.Bottom);

            int left = (int) Bounds.Left;
            int right = (int) Bounds.Right;
            int top = (int) Bounds.Top;
            int bottom = (int) Bounds.Bottom;

            //Debug.Log("Bounds: " + Bounds);

            Color[] pixels = GetPixels();
            //Debug.Log("pixels: " + pixels.Length);

            int count = 0;
            //Debug.Log(string.Format("yMax: {0}; yMin:{1}", yMax, yMin));
            DrawPixels(fillRect, bottom, top, left, right, hasStroke, pixels, count);

            SetPixels(pixels);
        }

        private static Color _transparent = new Color(1, 1, 1, 0); // transparent

        private void DrawPixels(Rectangle fillRect, int yMax, int yMin, int xMin, int xMax, bool hasStroke, Color[] pixels, int count)
        {
            //if (null == Fill)
            //    Fill = new Fill(new Color(1, 1, 1, 0)); // transparent

            for (int y = yMax; y > yMin; y--)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    _isFill = false;
                    _isStroke = false;

                    _point.X = x; 
                    _point.Y = y;

                    _isFill = fillRect.Contains(_point);

                    if (hasStroke && !_isFill)
                        _isStroke = Bounds.Contains(_point);

                    _color = _transparent; // new Color(1, 1, 1, 0); // transparent

                    if (_isFill && null != Fill) {
                        _color = CalculatePixelColor(Fill, y, x);
                    }
                    else if (_isStroke && null != Stroke) {
                        _color = CalculatePixelColor(Stroke, x, y);
                    }

                    pixels[count] = _color;

                    count++;
                }
            }
        }
    }
}