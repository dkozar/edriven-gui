/*using eDriven.Gui.Graphics.Base;
using UnityEngine;

namespace eDriven.Gui.Graphics
{
    public sealed class Spectrum : GraphicsBase
    {
        //All colors

        #region Util

        private static readonly Vector3[] Colors =
            {
                new Vector3(1.0f, 0.0f, 0.0f), //red
                new Vector3(1.0f, 165.0f/255.0f, 0.0f), //oragne
                new Vector3(1.0f, 1.0f, 0.0f), //yellow
                new Vector3(0.0f, 1.0f, 0.0f), //green
                new Vector3(0.0f, 0.0f, 1.0f), //blue
                new Vector3(75.0f/255.0f, 0.0f, 130.0f/255.0f), //indigo
                new Vector3(238.0f/255.0f, 130.0f/255.0f, 238.0f/255.0f), //violet
                new Vector3(1.0f, 1.0f, 1.0f), //white
                new Vector3(0.0f, 0.0f, 0.0f) //black
            };

        public static Color ColorFromPosition(Vector2 position, Vector2 size)
        {
            Vector3 pureColor = new Vector3();
            Vector3 finaleColor = new Vector3();
            float horizontalChunk = size.x/7;
            float verticalChunk = size.y/2;

            //Hue
            for (uint i = 0; i < 7; i++) //i < 7-1
            {
                if ((position.x >= horizontalChunk*i) && (position.x < horizontalChunk*(i + 1)))
                    pureColor = Vector3.Lerp(Colors[i], Colors[i + 1],
                                             (position.x - horizontalChunk*i)/horizontalChunk);
            }

            //Lightness
            if ((position.y >= verticalChunk) && (position.y < (verticalChunk*2)+1))
                finaleColor = Vector3.Lerp(pureColor, Colors[7], (position.y - verticalChunk)/verticalChunk);
            else
                finaleColor = Vector3.Lerp(Colors[8], pureColor, position.y/verticalChunk);

            return new Color(finaleColor.x, finaleColor.y, finaleColor.z, 1.0f);
        }

        public static Vector2 PositionFromColor(Color color, Vector2 size)
        {
            //predostavlja da se najslicnija boja nalazi na 0x,0y
            Vector2 similarPosition = new Vector2(0, 0);
            Color similarColor = ColorFromPosition(similarPosition, size);
            float similarityFactor =
                Mathf.Sqrt(Mathf.Pow(similarColor.r - color.r, 2) + Mathf.Pow(similarColor.g - color.g, 2) +
                           Mathf.Pow(similarColor.b - color.b, 2));

            //prolazi kroz cijelu bitmapu
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    similarColor = ColorFromPosition(new Vector2(x, y), size);
                    float localSimilarityFactor =
                        Mathf.Sqrt(Mathf.Pow(similarColor.r - color.r, 2) + Mathf.Pow(similarColor.g - color.g, 2) +
                                   Mathf.Pow(similarColor.b - color.b, 2));

                    if (localSimilarityFactor == 0) //ovo je tocno kada je pronadjena identicna boja trazenoj
                        return new Vector2(x, y);
                    
                    if (localSimilarityFactor < similarityFactor)
                        //ako je trenutna boja slicnija od zadnje zabiljezene
                    {
                        similarityFactor = localSimilarityFactor;
                        similarPosition = new Vector2(x, y);
                    }
                }
            }
            //na posljetku vraca najslicniju boju
            return similarPosition;
        }

        public static Texture2D GenerateTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] bitmap = new Color[width*height];

            for (int y = 0; y < texture.height; y++)
                for (int x = 0; x < texture.width; x++)
                    bitmap[x + (width*y)] = ColorFromPosition(new Vector2(x, y), new Vector2(width, height));

            texture.SetPixels(bitmap);
            texture.Apply();
            return texture;
        }

        public static Texture2D WhiteTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] bitmap = new Color[width * height];

            for (int y = 0; y < texture.height; y++)
                for (int x = 0; x < texture.width; x++)
                    bitmap[x + (width * y)] = Color.white;

            texture.SetPixels(bitmap);
            texture.Apply();
            return texture;
        }

        #endregion

        #region Constructor

        public Spectrum(int x, int y, int width, int height, params GraphicOption[] options)
            : base(x, y, width, height, options)
        {
        }

        public Spectrum(int width, int height, params GraphicOption[] options)
            : base(width, height, options)
        {
        }

        #endregion

        public override void Draw()
        {
            //Debug.Log("Drawing spectrum");

            //base.Draw();

            //var time1 = DateTime.Now;
            //Debug.Log("time1: " + time1);

            int xMin = (int)Bounds.Left;
            int xMax = (int)Bounds.Right;
            int yMin = (int)Bounds.Top;
            int yMax = (int)Bounds.Bottom;

            int w = (int) Bounds.Width;
            int h = (int) Bounds.Height;

            Color[] pixels = GetPixels();

            for (int y = yMin; y < yMax; y++)
                for (int x = xMin; x < xMax; x++)
                    pixels[x + (w * y)] = ColorFromPosition(new Vector2(x, y), new Vector2(w, h));

            SetPixels(pixels);

            //var time2 = DateTime.Now;
            //Debug.Log("time2: " + time2);

            //Debug.Log(string.Format("Spectrum rendering time: {0} ms", (time2 - time1).TotalMilliseconds));
        }
    }
}*/