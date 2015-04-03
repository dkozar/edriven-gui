using UnityEngine;

namespace eDriven.Gui.Util
{
    public class ColorMixer
    {
        private float _red = 1f;
        public float Red
        {
            get
            {
                return _red;
            }
            set
            {
                if (value != _red)
                {
                    _red = value;
                    RefreshState();
                }
            }
        }

        private float _green = 1f;
        public float Green
        {
            get
            {
                return _green;
            }
            set
            {
                if (value != _green)
                {
                    _green = value;
                    RefreshState();
                }
            }
        }

        private float _blue = 1f;
        public float Blue
        {
            get
            {
                return _blue;
            }
            set
            {
                if (value != _blue)
                {
                    _blue = value;
                    RefreshState();
                }
            }
        }

        private float _alpha = 1f;
        public float Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                if (value != _alpha)
                {
                    _alpha = value;
                    RefreshState();
                }
            }
        }

        public ColorMixer()
        {
        }

        public ColorMixer(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public ColorMixer(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        private void RefreshState()
        {
            _isSet = _red < 1f || _green < 1f || _blue < 1f;
        }

        private bool _isSet;
        public bool IsSet
        {
            get
            {
                return _isSet;
            }
        }

        public static ColorMixer FromHex(int hexColor)
        {
            ColorMixer c = new ColorMixer
                             {
                                 Red = (float) ((hexColor & 16711680) >> 16)/255,
                                 Green = (float) ((hexColor & 65280) >> 8)/255,
                                 Blue = (float) (hexColor & 255)/255
                             };
            return c;
        }

        public static ColorMixer FromHexAndAlpha(int hexColor, float alpha)
        {
            ColorMixer c = new ColorMixer
            {
                Red = (float)((hexColor & 16711680) >> 16) / 255,
                Green = (float)((hexColor & 65280) >> 8) / 255,
                Blue = (float)(hexColor & 255) / 255,
                Alpha = alpha
            };
            return c;
        }

        public Color ToColor()
        {
            return new Color(Red, Green, Blue, Alpha);
        }

        public static ColorMixer FromColor(Color c)
        {
            return new ColorMixer(c.r, c.g, c.b, c.a);
        }

        public override string ToString()
        {
            return string.Format("Red: {0}, Green: {1}, Blue: {2}, Alpha: {3}", Red, Green, Blue, Alpha);
        }
    }
}