/*using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class ControlGroupBackgroundStyle
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

        private ControlGroupBackgroundStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 3;

        private static void Initialize()
        {
            _instance.name = "ControlGroupBackgroundStyle";

            _style = new ProgramaticStyle
                         {
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter/*,
                             Font = FontMapper.GetDefault()#1#
                         };

            const int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            _style.Padding = new RectOffset(2, 2, 2, 2);

            //_style.FontSize = 8;
            //_style.NormalTextColor = new Color(0.85f, 0.85f, 0.85f, 1);
            _style.NormalGraphics = new Rect(w, w, new Fill(ColorMixer.FromHex(0xBBD4F6).ToColor())); // new Fill(new Color(0.8f, 0.8f, 0.8f, 1)));

            _style.Commit();

        }
    }
}*/