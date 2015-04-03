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
using eDriven.Gui.Graphics.Base;
using UnityEngine;

namespace eDriven.Gui.Graphics
{
    public sealed class Ellipse : GraphicsBase
    {
        private bool _isFill;
        private bool _isStroke;

        private Color? _color;

        #region Constructor

        public Ellipse()
        {
        }

        public Ellipse(params GraphicOption[] options) : base(options)
        {
        }

        public Ellipse(IFill fill, params GraphicOption[] options) : base(fill, options)
        {
        }

        public Ellipse(IStroke stroke, params GraphicOption[] options) : base(stroke, options)
        {
        }

        public Ellipse(IFill fill, IStroke stroke) : base(fill, stroke)
        {
        }

        public Ellipse(int width, int height, params GraphicOption[] options) : base(width, height, options)
        {
        }

        public Ellipse(int width, int height, IStroke stroke, params GraphicOption[] options) : base(width, height, stroke, options)
        {
        }

        public Ellipse(int width, int height, IFill fill, params GraphicOption[] options) : base(width, height, fill, options)
        {
        }

        public Ellipse(int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(width, height, fill, stroke, options)
        {
        }

        public Ellipse(int x, int y, int width, int height, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, stroke, options)
        {
        }

        public Ellipse(int x, int y, int width, int height, IFill fill, params GraphicOption[] options) : base(x, y, width, height, fill, options)
        {
        }

        public Ellipse(int x, int y, int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, fill, stroke, options)
        {
        }

        public Ellipse(int x, int y, int width, int height, params GraphicOption[] options) : base(x, y, width, height, options)
        {
        }

        public Ellipse(IFill fill, IStroke stroke, params GraphicOption[] options) : base(fill, stroke, options)
        {
        }

        #endregion

        public override void Draw()
        {
            //Debug.Log("Drawing ellipse");

            if (null == Fill && null == Stroke)
                return;

            base.Draw();

            Rectangle b = RenderingBounds;

            //Debug.Log("RenderingBounds: " + RenderingBounds);

            int xMin = (int)b.Left;
            int xMax = (int)b.Right;
            int yMin = (int)b.Top;
            int yMax = (int)b.Bottom;

            int width = (int)b.Width;
            int height = (int)b.Height;

            //Color[] pixels = GetPixels(texture);

            int radius = width / 2;
            int fillRadius = radius;

            bool hasStroke = false;
            if (null != Stroke)
            {
                fillRadius -= Stroke.Border.Maximum;
                hasStroke = true;
            }

            int xCenter = xMin + width / 2;
            int yCenter = yMin + height / 2;

            //int count = 0;
            for (int y = yMax; y > yMin; y--)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    _isFill = false;
                    _isStroke = false;

                    int rX = x - xCenter;
                    int rY = y - yCenter;

                    _isFill = Math.Sqrt(rX * rX + rY * rY) < fillRadius;

                    if (hasStroke && !_isFill)
                        _isStroke = Math.Sqrt(rX * rX + rY * rY) < radius;

                    _color = null;
                    
                    //if (_isFill && null != Fill)
                    //    _color = Fill.Color;
                    //else if (_isStroke && null != Stroke)
                    //    _color = Stroke.Color;

                    if (_isFill && null != Fill)
                        _color = CalculatePixelColor(Fill, y, x);
                    else if (_isStroke && null != Stroke)
                        _color = CalculatePixelColor(Stroke, x, y);

                    if (null != _color)
                        Texture.SetPixel(x, y, (Color) _color);

                    //pixels[count] = _color;
                    //count++;
                }
            }

            //SetPixels(texture, pixels);
        }
    }
}
