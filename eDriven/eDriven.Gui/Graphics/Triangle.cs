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
using eDriven.Gui.Graphics.Base;

namespace eDriven.Gui.Graphics
{
    public sealed class Triangle : GraphicsBase
    {
        #region Constructor

        public Triangle(IFill fill, params GraphicOption[] options) : base(fill, options)
        {
            
        }

        #endregion

        public int Pixels = 3;

        public TriangleDirection Direction = TriangleDirection.Up;

        public override void Draw()
        {
            //Debug.Log("Drawing fractal: " + Bounds);

            //Pixels = (int) (Math.Floor(_backgroundSize.X) - 6); // 3 pixels on each side

            base.Draw();

            //Color[] pixels = GetPixels();

            int xMin = (int)Bounds.Left;
            int xMax = (int)Bounds.Right;
            int yMin = (int)Bounds.Top;
            int yMax = (int) Bounds.Bottom;

            //Debug.Log("yMin: " + yMin + "; yMax: " + yMax);

            int xMiddle = (int) Math.Floor((float)(xMax - xMin)/2);
            int yMiddle = (int)Math.Floor((float)(yMax - yMin) / 2);

            //Debug.Log("xMiddle: " + xMiddle + "; yMiddle: " + yMiddle);

            int count = 0;
            int xStart, xEnd, yStart, yEnd;
            int halfPixels = (int) Math.Floor((float) Pixels/2);

            switch (Direction)
            {
                case TriangleDirection.Up:

                    //yMax -= 9 + Pixels - 7;
                    yStart = yMiddle + halfPixels;
                    yEnd = yStart - Pixels;
                    for (int y = yStart; y > yEnd; y--)
                    {
                        for (int x = xMiddle - count; x <= xMiddle + count; x++)
                        {
                            //Debug.Log("x: " + x + "; y: " + y);
                            Texture.SetPixel(x, y, Fill.Color);
                        }

                        count++;

                        if (count > Pixels)
                            break;
                    }
                    break;

                case TriangleDirection.Down:

                    yEnd = yMiddle + halfPixels;
                    yStart = yEnd - Pixels;
                    for (int y = yStart; y < yEnd; y++)
                    {
                        for (int x = xMiddle - count; x <= xMiddle + count; x++)
                            Texture.SetPixel(x, y, Fill.Color);

                        count++;

                        if (count > Pixels)
                            break;
                    }
                    break;

                case TriangleDirection.Left:

                    xStart = xMiddle - halfPixels;
                    xEnd = xStart + Pixels;
                    for (int x = xStart; x < xEnd; x++)
                    {
                        for (int y = yMiddle - count; y <= yMiddle + count; y++)
                            Texture.SetPixel(x, y, Fill.Color);

                        count++;

                        if (count > Pixels)
                            break;
                    }
                    break;

                case TriangleDirection.Right:

                    xEnd = xMiddle - halfPixels;
                    xStart = xEnd + Pixels;
                    for (int x = xStart; x > xEnd; x--)
                    {
                        for (int y = yMiddle - count; y <= yMiddle + count; y++)
                            Texture.SetPixel(x, y, Fill.Color);

                        count++;

                        if (count > Pixels)
                            break;
                    }
                    break;
            }

            

            //SetPixels(pixels);
        }
    }

    public enum TriangleDirection
    {
        Up, Down, Left, Right
    }
}
