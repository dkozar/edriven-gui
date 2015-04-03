using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ResizableBorderStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting FocusIndicatorStyle instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private ResizableBorderStyle()
        {
            // constructor is protected
        }

        #endregion

        public static int BorderWidth = 8;
        public static Color BorderColor = new Color(0.25f, 0.25f, 0.25f, 1);
        
        private static ProgramaticStyle _style;

        public static int Divider = 3;
        
        //private const int Weight = 2;

        public static void Draw()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //Debug.Log("FocusIndicatorStyle initializer");

            _style = new ProgramaticStyle();
            _style.Style = _instance;
            
            // NOTE: we don't need font if we are not calculating the styled label size!
            //_style.Font = FontMapper.GetDefault();
            
            //_style.Alignment = TextAnchor.MiddleLeft;
            //_style.Padding = new RectOffset(10, 10, 0, 0);

            int w = (BorderWidth + 1) * 2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            int p = BorderWidth + 1;
            _style.Border = new RectOffset(p, p, p, p); // new RectOffset(Divider, Divider, 0, 0);
            //_style.Overflow = new RectOffset(BorderWidth, BorderWidth, BorderWidth, BorderWidth);

            //_style.FontSize = 30;
            //_style.NormalTextColor = Color.grey;
            _style.NormalGraphics = new Rect(w, w,
                new Stroke(BorderWidth)
                    {
                        Color = BorderColor,
                        DrawFunction = delegate(int x, int y, Color c)
                        {
                            if (x < BorderWidth && y < BorderWidth || x > w - BorderWidth && y > w - BorderWidth)
                                return (x + y) % Divider == 0 ? c : Color.white;

                            if (x > w - BorderWidth && y < BorderWidth || x < BorderWidth && y > w - BorderWidth)
                                return (y - x) % Divider == 0 ? c : Color.white;

                            if (x < BorderWidth || x > w - BorderWidth)
                            {
                                return x % 2 == 1 ? c : Color.white;
                            }

                            if (y < BorderWidth || y > w - BorderWidth)
                            {
                                return y % 2 == 0 ? c : Color.white;
                            }

                            return new Color(1, 1, 1, 0); //Color.white;
                        }
                    }
                                             //new Stroke(BorderColor, BorderWidth, 
                                             //    delegate(int x, int y, Color c)
                                             //    {
                                             //        if (x < BorderWidth && y < BorderWidth || x > w - BorderWidth && y > w - BorderWidth)
                                             //            return (x + y) % Divider == 0 ? c : Color.white;

                                             //        if (x > w - BorderWidth && y < BorderWidth || x < BorderWidth && y > w - BorderWidth)
                                             //            return (y - x) % Divider == 0 ? c : Color.white;

                                             //        if (x < BorderWidth || x > w - BorderWidth)
                                             //        {
                                             //            return x % 2 == 1 ? c : Color.white;
                                             //        }

                                             //        if (y < BorderWidth || y > w - BorderWidth)
                                             //        {
                                             //            return y % 2 == 0 ? c : Color.white;
                                             //        }
                                                     
                                             //        return new Color(1, 1, 1, 0); //Color.white;
                                             //    }
                                             //)
                );

            _style.Commit();
        }
    }
}