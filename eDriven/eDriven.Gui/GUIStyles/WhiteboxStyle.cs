/*using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles.Helper;
using UnityEngine;

namespace eDriven.Gui.GUIStyles
{
    public class WhiteboxStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting WhiteboxStyle instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private WhiteboxStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            const int w = (Weight + 1) * 2;
            //Debug.Log("WhiteboxStyle initializer");

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Padding = new RectOffset(6, 6, 4, 4),
                             Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1),
                             NormalGraphics = new Graphics.Rect(w, w, new Fill(ScrollbarHelper.BackgroundColor), new Stroke(1) { Color = ScrollbarHelper.StrokeColor }),
                         };
            _style.Commit();
        }
    }
}*/