using eDriven.Gui.Mappers;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect = eDriven.Gui.Graphics.Rect;

namespace eDriven.Playground.eDrivenSite
{
    public class GreenButtonStyle
    {
        internal static readonly Color Green = ColorMixer.FromHex(0x3FFFAF).ToColor();

        #region Quasi-Singleton

        private static GUIStyle _instance;
        public static GUIStyle Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.Log("Getting ImageOnlyButtonStyle instance");

                    _instance = new GUIStyle();
                    Initialize();
                }
                return _instance;
            }
        }

        private GreenButtonStyle()
        {
            // constructor is protected
        }

        #endregion

        private static ProgramaticStyle _style;

        private const int Weight = 1;

        private static void Initialize()
        {
            _instance.name = "GreenButtonStyle";
            _instance.font = FontMapper.GetDefault().Font;

            _style = new ProgramaticStyle
                         {
                             Font = FontMapper.GetDefault().Font,
                             Style = _instance,
                             Alignment = TextAnchor.MiddleCenter,
                             Padding = new RectOffset(6, 6, 3, 3)
                         };

            const int w = (Weight + 1) * 2;

            //_style.FixedWidth = 100;
            //_style.FixedHeight = 100;
            _style.Border = new RectOffset(Weight + 1, Weight + 1, Weight + 1, Weight + 1);

            //_style.FontSize = 8;
            _style.NormalTextColor = Color.black;
            _style.NormalGraphics = new Rect(w, w,
                                             new Fill(Green),
                                             new Stroke(Weight) { Color = Color.black }
                                    );

            _style.HoverTextColor = Color.black;
            _style.HoverGraphics = new Rect(w, w,
                                             new Fill(ColorMixer.FromHex(0x14FF99).ToColor()),
                                             new Stroke(Weight) { Color = Color.black }
                                    );

            _style.Commit();
        }
    }
}