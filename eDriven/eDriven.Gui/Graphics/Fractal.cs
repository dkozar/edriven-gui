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


using eDriven.Gui.Graphics.Base;
using UnityEngine;

namespace eDriven.Gui.Graphics
{
    public sealed class Fractal : GraphicsBase
    {
        #region Constructor

        public Fractal()
        {
        }

        public Fractal(params GraphicOption[] options) : base(options)
        {
        }

        public Fractal(IFill fill, params GraphicOption[] options) : base(fill, options)
        {
        }

        public Fractal(IStroke stroke, params GraphicOption[] options) : base(stroke, options)
        {
        }

        public Fractal(IFill fill, IStroke stroke) : base(fill, stroke)
        {
        }

        public Fractal(int width, int height, params GraphicOption[] options) : base(width, height, options)
        {
        }

        public Fractal(int width, int height, IStroke stroke, params GraphicOption[] options) : base(width, height, stroke, options)
        {
        }

        public Fractal(int width, int height, IFill fill, params GraphicOption[] options) : base(width, height, fill, options)
        {
        }

        public Fractal(int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(width, height, fill, stroke, options)
        {
        }

        public Fractal(int x, int y, int width, int height, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, stroke, options)
        {
        }

        public Fractal(int x, int y, int width, int height, IFill fill, params GraphicOption[] options) : base(x, y, width, height, fill, options)
        {
        }

        public Fractal(int x, int y, int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, fill, stroke, options)
        {
        }

        public Fractal(int x, int y, int width, int height, params GraphicOption[] options) : base(x, y, width, height, options)
        {
        }

        public Fractal(IFill fill, IStroke stroke, params GraphicOption[] options) : base(fill, stroke, options)
        {
        }

        #endregion

        public override void Draw()
        {
            //Debug.Log("Drawing fractal: " + Bounds);

            base.Draw();

            Color[] pixels = GetPixels();

            int xMin = (int)Bounds.Left;
            int xMax = (int)Bounds.Right;
            int yMin = (int)Bounds.Top;
            int yMax = (int) Bounds.Bottom;

            int count = 0;
            // Fill the texture with Sierpinski's fractal pattern!
            for (int y = yMax; y > yMin; y--)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    //var color = Color.red;
                    var color = (x & y) != 0 ? Color.white : Color.gray;
                    //texture.SetPixel(x, y, color);
                    pixels[count] = color;
                    count++;
                }
            }

            SetPixels(pixels);
        }
    }
}
