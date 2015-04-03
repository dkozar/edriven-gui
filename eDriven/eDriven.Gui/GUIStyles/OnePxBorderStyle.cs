using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class OnePxBorderStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting RectButtonSkin instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private OnePxBorderStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private static void Initialize()
        {
            _instance.name = "OnePxBorderStyle";

            _style = new ProgramaticStyle {Style = _instance};

            var border = new BorderMetrics(1); // 1px border
            var w = border.GetRenderingWidth();
            var h = border.GetRenderingHeight();
            //Debug.Log("w: " + w + ", h:" + h);

            _style.Border = border.ToGUIStyleBorder();
            _style.NormalGraphics = new Rect(w, h, new Stroke(border) { Color = Color.white });
            _style.Commit();
        }
    }
}