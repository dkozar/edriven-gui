using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles
{
    public class PanelBackgroundStyle
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

        private PanelBackgroundStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 3;

        private static void Initialize()
        {
            _instance.name = "PanelBackgroundStyle";

            _style = new ProgramaticStyle {Style = _instance, Alignment = TextAnchor.MiddleCenter};

            //_style.Font = FontMapper.GetDefault();

            const int w = (Weight + 1) * 2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            _style.Padding = new RectOffset(2, 2, 2, 2);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.white;
            //_style.NormalGraphics = new Rect(w, w, new Fill(RgbColor.FromHex(0xBBD4F6).ToColor())/*, new Stroke(Color.grey, Weight)*/); //new Color(1, 1, 1, 1)
            _style.NormalGraphics = new Rect(w, w, new Fill(new Color(1, 1, 1, 1)));

            _style.Commit();

        }
    }
}