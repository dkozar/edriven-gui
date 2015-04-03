/*
using eDriven.Gui.Graphics.Base;
using UnityEngine;

namespace eDriven.Gui.Graphics
{
    public sealed class Noise : GraphicsBase
    {
        #region Constructor

        public Noise()
        {
        }

        public Noise(params GraphicOption[] options) : base(options)
        {
        }

        public Noise(IFill fill, params GraphicOption[] options) : base(fill, options)
        {
        }

        public Noise(IStroke stroke, params GraphicOption[] options) : base(stroke, options)
        {
        }

        public Noise(IFill fill, IStroke stroke) : base(fill, stroke)
        {
        }

        public Noise(int width, int height, params GraphicOption[] options) : base(width, height, options)
        {
        }

        public Noise(int width, int height, IStroke stroke, params GraphicOption[] options) : base(width, height, stroke, options)
        {
        }

        public Noise(int width, int height, IFill fill, params GraphicOption[] options) : base(width, height, fill, options)
        {
        }

        public Noise(int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(width, height, fill, stroke, options)
        {
        }

        public Noise(int x, int y, int width, int height, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, stroke, options)
        {
        }

        public Noise(int x, int y, int width, int height, IFill fill, params GraphicOption[] options) : base(x, y, width, height, fill, options)
        {
        }

        public Noise(int x, int y, int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options) : base(x, y, width, height, fill, stroke, options)
        {
        }

        public Noise(int x, int y, int width, int height, params GraphicOption[] options) : base(x, y, width, height, options)
        {
        }

        public Noise(IFill fill, IStroke stroke, params GraphicOption[] options) : base(fill, stroke, options)
        {
        }

        #endregion

        public override void Draw()
        {
            Debug.Log("Drawing noise: " + Texture.width + ", " + Texture.height);

            base.Draw();

            int xMin = (int)Bounds.Left;
            int xMax = (int)Bounds.Right;
            int yMin = (int)Bounds.Top;
            int yMax = (int)Bounds.Bottom;

            Color[] pixels = GetPixels();

            int count = 0;
            for (int y = yMax; y > yMin; y--)
            {
                for (int x = xMin; x < xMax; x++)
                {

                    float r = Random.value;
                    Color color = new Color(r, r, r);
                    pixels[count] = color;
                    count++;
                }
            }

            SetPixels(pixels);
        }
    }
}
*/
