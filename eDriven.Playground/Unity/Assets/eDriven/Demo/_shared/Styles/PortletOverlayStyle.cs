using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Playground.eDrivenSite
{
    public class PortletOverlayStyle
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

        private PortletOverlayStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _instance.name = "PortletOverlayStyle";

            _style = new ProgramaticStyle {Style = _instance};

            //_style.Alignment = TextAnchor.MiddleCenter;

            //_style.Font = FontMapper.GetDefault();

            const int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            //_style.Padding = new RectOffset(2, 2, 2, 2);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.white;
            _style.NormalGraphics = new Rect(w, w, new Fill(new Color(1, 1, 1, 0)), new Stroke(Weight) { Color = ColorMixer.FromHex(0xbf9eff/*0xc0c0c0*//*0x214078*/).ToColor() });

            //_style.HoverGraphics = new Rect(w, w, new Fill(new Color(1, 1, 1, 0)), new Stroke(new Color(0.2f, 0.2f, 0.2f, 1), Weight));

            //_style.ActiveGraphics = new Rect(w, w, new Fill(new Color(1, 1, 1, 0)), new Stroke(new Color(0.1f, 0.1f, 0.1f, 1), Weight));

            _style.Commit();

        }
    }
}