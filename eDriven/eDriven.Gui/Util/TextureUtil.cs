using System;
using eDriven.Gui.Geom;
using UnityEngine;

namespace eDriven.Gui.Util
{
    /// <summary>
    /// Hansles various texture tasks
    /// </summary>
    public static class TextureUtil
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// Evaluates the 9-patch metadata from the actual texture
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static EdgeMetrics EvaluatePatch9Border(Texture texture)
        {
            if (null == texture)
            {
                // no texture found
                return new EdgeMetrics();
            }

            Texture2D texture2D = texture as Texture2D;
            if (null == texture2D)
            {
                // not a Texture2D
                return new EdgeMetrics();
            }

            int w = texture.width;
            int h = texture.height;

            if (w <= 1)
                throw new Exception("Not a valid texture width");

            if (h <= 1)
                throw new Exception("Not a valid texture height");

            //RectOffset output = new RectOffset();
            int? left = null;
            int? right = null;
            int? top = null;
            int? bottom = null;

            try
            {
                // sanity check
                Color pixel = texture2D.GetPixel(0, h - 1);
                if (pixel.a > 0)
                {
                    throw new Exception("When using 9-patch, the top-left pixel needs to be transparent");
                }

                // read 1st row from left
                for (int i = 0; i < w; i++)
                {
                    pixel = texture2D.GetPixel(i, h - 1);
                    if (pixel.a > 0)
                    {
                        left = i + 1;
                        break;
                    }
                }
                if (null == left)
                    throw new Exception("9-patch: Couldn't find the non transparent pixel in the first row (left)");

                // read 1st row from right
                for (int i = w - 1; i >= 0; i--)
                {
                    pixel = texture2D.GetPixel(i, h - 1);
                    if (pixel.a > 0)
                    {
                        right = w - i;// + 1;
                        break;
                    }
                }
                if (null == right)
                    throw new Exception("9-patch: Couldn't find the non transparent pixel in the first row (right)");

                // read 1st column from top
                for (int i = 0; i < h; i++)
                {
                    pixel = texture2D.GetPixel(0, h - i);
                    if (pixel.a > 0)
                    {
                        top = i + 1;
                        break;
                    }
                }
                if (null == top)
                    throw new Exception("9-patch: Couldn't find the non transparent pixel in the first column (top)");

                // read 1st column from bottom
                for (int i = 0; i < h; i++)
                {
                    pixel = texture2D.GetPixel(0, i);
                    if (pixel.a > 0)
                    {
                        bottom = i + 1;
                        break;
                    }
                }
                if (null == bottom)
                    throw new Exception("9-patch: Couldn't find the non transparent pixel in the first column (bottom)");

            }
            catch (UnityException ex)
            {
                throw new Exception("Couldn't read the texture data. Perhaps it is not set as readable? (see Texture importer settings)", ex);
            }

            EdgeMetrics output = new EdgeMetrics((float)left, (float)right, (float)top, (float)bottom);

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("9-patch: " + output);
            }
#endif
            return output;
        }
    }
}
