using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class FocusIndicatorStyle
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

        private FocusIndicatorStyle()
        {
            // constructor is protected
        }

        #endregion

        public static int BorderWidth = 3;
        public static Color BorderColor = ColorMixer.FromHexAndAlpha(0x00aeff, 0.6f).ToColor(); // 0x3FFFAF //Color.yellow; // new Color(1, 0.85f, 0, 0.75f);
        
        private static ProgramaticStyle _style;

        public static void Draw()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //Debug.Log("FocusIndicatorStyle initializer");

            _style = new ProgramaticStyle {Style = _instance};

            // NOTE: we don't need font if we are not calculating the styled label size!
            
            int w = (BorderWidth + 1) * 2;

            int p = BorderWidth + 1;
            _style.Border = new RectOffset(p, p, p, p);
            _style.Overflow = new RectOffset(BorderWidth - 1, BorderWidth - 1, BorderWidth - 1, BorderWidth - 1);

            _style.NormalGraphics = new Rect(w, w, new Stroke(BorderWidth) { Color = BorderColor, Border = new BorderMetrics(BorderWidth) });

            _style.Commit();
        }
    }
}