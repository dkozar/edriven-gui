using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace Assets.eDriven.Demo.Styles
{
    public class BlueLabelStyle
    {
        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                //Debug.Log("Getting FormItemLabelSkin instance");

                if (_instance == null)
                {
                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private BlueLabelStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 2;

        private static void Initialize()
        {
            _style = new ProgramaticStyle
            {
                Style = _instance,
                Alignment = TextAnchor.MiddleLeft,
                Padding = new RectOffset(6, 6, 4, 4),
                Font = FontMapper.GetWithFallback("pixel").Font
            };

            const int w = (Weight + 1)*2;

            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);
            _style.NormalTextColor = Color.white;
            _style.NormalGraphics = new Rect(w, w, new Fill(ColorMixer.FromHex(0x1261c1).ToColor()));
            
            _style.Commit();
        }
    }
}